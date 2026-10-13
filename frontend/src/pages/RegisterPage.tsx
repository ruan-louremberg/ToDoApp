import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export function RegisterPage() {
  const { register, error: sessionError } = useAuth();
  const navigate = useNavigate();
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSaving(true);
    setError(null);

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

        <form className="auth-form" onSubmit={handleSubmit}>
          {sessionError && (
            <p className="auth-form__error" role="alert">{sessionError}</p>
          )}
          <label htmlFor="register-name">Nome</label>
          <input
            id="register-name"
            type="text"
            autoComplete="name"
            maxLength={120}
            value={name}
            onChange={(event) => setName(event.target.value)}
            required
          />
          <label htmlFor="register-email">E-mail</label>
          <input
            id="register-email"
            type="email"
            autoComplete="email"
            maxLength={320}
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            required
          />
          <label htmlFor="register-password">Senha</label>
          <input
            id="register-password"
            type="password"
            autoComplete="new-password"
            minLength={8}
            maxLength={100}
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            required
          />
          <span className="auth-form__hint">Use pelo menos 8 caracteres.</span>
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
