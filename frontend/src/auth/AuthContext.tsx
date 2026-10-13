import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import {
  getCurrentUser,
  loginUser,
  logoutUser,
  registerUser,
  type AuthUser,
  type LoginRequest,
  type RegisterRequest,
} from "../api/auth";
import { ApiError } from "../api/client";

interface AuthContextValue {
  user: AuthUser | null;
  loading: boolean;
  error: string | null;
  login: (request: LoginRequest) => Promise<void>;
  register: (request: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;

    getCurrentUser()
      .then((currentUser) => {
        if (active) setUser(currentUser);
      })
      .catch((requestError: unknown) => {
        if (!active) return;

        if (requestError instanceof ApiError && requestError.status === 401) {
          setUser(null);
          return;
        }

        setError(
          requestError instanceof Error
            ? requestError.message
            : "Não foi possível verificar sua sessão."
        );
      })
      .finally(() => {
        if (active) setLoading(false);
      });

    return () => {
      active = false;
    };
  }, []);

  const login = useCallback(async (request: LoginRequest) => {
    const currentUser = await loginUser(request);
    setUser(currentUser);
    setError(null);
  }, []);

  const register = useCallback(async (request: RegisterRequest) => {
    await registerUser(request);
    const currentUser = await loginUser({
      email: request.email,
      password: request.password,
    });
    setUser(currentUser);
    setError(null);
  }, []);

  const logout = useCallback(async () => {
    try {
      await logoutUser();
    } catch (requestError) {
      if (!(requestError instanceof ApiError && requestError.status === 401)) {
        throw requestError;
      }
    }

    setUser(null);
    setError(null);
  }, []);

  const value = useMemo(
    () => ({ user, loading, error, login, register, logout }),
    [user, loading, error, login, register, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth deve ser usado dentro de AuthProvider.");
  }

  return context;
}
