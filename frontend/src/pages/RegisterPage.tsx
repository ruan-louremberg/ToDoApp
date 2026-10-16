import { useState, type FormEvent } from "react";
import { Eye, EyeOff } from "lucide-react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export function RegisterPage() {
  const { register, error: sessionError } = useAuth();
  const navigate = useNavigate();
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    if (password !== confirmPassword) {
      setError("As senhas informadas não coincidem.");
      return;
    }

    setSaving(true);
    try {
      await register({ name, email, password });
      navigate("/tasks", { replace: true });
    } catch (requestError) {
      setError(
        requestError instanceof Error
          ? requestError.message
          : "Não foi possível criar sua conta."
      );
    } finally {
      setSaving(false);
    }
  }

  return (
    <main className="auth-page">
      <section className="auth-card" aria-labelledby="register-title">
        <Link className="auth-brand" to="/register" aria-label="todo.">
          <span className="header__brand-mark" aria-hidden="true">T</span>
          <span>todo<span className="header__brand-accent">.</span></span>
        </Link>
        <p className="auth-card__eyebrow">COMECE POR AQUI</p>
        <h1 id="register-title">Crie sua conta</h1>
        <p className="auth-card__description">
          Organize suas tarefas em um espaço só seu.
        </p>

        <form className="auth-form" autoComplete="off" onSubmit={handleSubmit}>
          {sessionError && (
            <p className="auth-form__error" role="alert">{sessionError}</p>
          )}
          <label htmlFor="register-name">Nome</label>
          <input
            id="register-name"
            type="text"
            autoComplete="off"
            maxLength={120}
            value={name}
            onChange={(event) => setName(event.target.value)}
            required
          />
          <label htmlFor="register-email">E-mail</label>
          <input
            id="register-email"
            type="email"
            autoComplete="off"
            maxLength={320}
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            required
          />
          <label htmlFor="register-password">Senha</label>
          <div className="auth-password-field">
            <input
              id="register-password"
              type={showPassword ? "text" : "password"}
              autoComplete="off"
              minLength={8}
              maxLength={100}
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
          <span className="auth-form__hint">Use pelo menos 8 caracteres.</span>
          <label htmlFor="register-confirm-password">Confirme a senha</label>
          <div className="auth-password-field">
            <input
              id="register-confirm-password"
              type={showConfirmPassword ? "text" : "password"}
              autoComplete="new-password"
              minLength={8}
              maxLength={100}
              value={confirmPassword}
              onChange={(event) => setConfirmPassword(event.target.value)}
              required
            />
            <button
              className="auth-password-field__toggle"
              type="button"
              aria-label={showConfirmPassword ? "Ocultar confirmação da senha" : "Mostrar confirmação da senha"}
              aria-pressed={showConfirmPassword}
              onClick={() => setShowConfirmPassword((visible) => !visible)}
            >
              {showConfirmPassword ? <EyeOff size={18} /> : <Eye size={18} />}
            </button>
          </div>
          {error && <p className="auth-form__error" role="alert">{error}</p>}
          <button type="submit" disabled={saving}>
            {saving ? "Criando conta..." : "Criar conta"}
          </button>
        </form>

        <p className="auth-card__switch">
          Já tem uma conta? <Link to="/login">Entrar</Link>
        </p>
      </section>
    </main>
  );
}
