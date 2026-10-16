import { useState, type FormEvent } from "react";
import { Eye, EyeOff } from "lucide-react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export function LoginPage() {
  const { login, error: sessionError } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSaving(true);
    setError(null);

    try {
      await login({ email, password });
      navigate("/tasks", { replace: true });
    } catch (requestError) {
      setError(
        requestError instanceof Error
          ? requestError.message
          : "Não foi possível entrar."
      );
    } finally {
      setSaving(false);
    }
  }

  return (
    <main className="auth-page">
      <section className="auth-card" aria-labelledby="login-title">
        <Link className="auth-brand" to="/login" aria-label="todo.">
          <span className="header__brand-mark" aria-hidden="true">T</span>
          <span>todo<span className="header__brand-accent">.</span></span>
        </Link>
        <p className="auth-card__eyebrow">BEM-VINDO DE VOLTA</p>
        <h1 id="login-title">Entre na sua conta</h1>
        <p className="auth-card__description">
          Acesse suas tarefas e continue de onde parou.
        </p>

        <form className="auth-form" onSubmit={handleSubmit}>
          {sessionError && (
            <p className="auth-form__error" role="alert">{sessionError}</p>
          )}
          <label htmlFor="login-email">E-mail</label>
          <input
            id="login-email"
            type="email"
            autoComplete="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            required
          />
          <label htmlFor="login-password">Senha</label>
          <div className="auth-password-field">
            <input
              id="login-password"
              type={showPassword ? "text" : "password"}
              autoComplete="current-password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              required
            />
            <button
              className="auth-password-field__toggle"
              type="button"
              aria-label={showPassword ? "Ocultar senha" : "Mostrar senha"}
              aria-pressed={showPassword}
              onClick={() => setShowPassword((visible) => !visible)}
            >
              {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
            </button>
          </div>
          <Link className="auth-form__forgot-link" to="/forgot-password">
            Esqueci minha senha
          </Link>
          {error && <p className="auth-form__error" role="alert">{error}</p>}
          <button type="submit" disabled={saving}>
            {saving ? "Entrando..." : "Entrar"}
          </button>
        </form>

        <p className="auth-card__switch">
          Ainda não tem conta? <Link to="/register">Criar conta</Link>
        </p>
      </section>
    </main>
  );
}
