import { useState, type FormEvent } from "react";
import { Link } from "react-router-dom";
import { requestPasswordReset } from "../api/auth";

export function ForgotPasswordPage() {
  const [email, setEmail] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSaving(true);
    setError(null);
    setMessage(null);

    try {
      const response = await requestPasswordReset({ email });
      setMessage(response.message);
    } catch (requestError) {
      setError(
        requestError instanceof Error
          ? requestError.message
          : "Não foi possível solicitar a redefinição."
      );
    } finally {
      setSaving(false);
    }
  }

  return (
    <main className="auth-page">
      <section className="auth-card" aria-labelledby="forgot-password-title">
        <Link className="auth-brand" to="/login" aria-label="todo.">
          <span className="header__brand-mark" aria-hidden="true">T</span>
          <span>todo<span className="header__brand-accent">.</span></span>
        </Link>
        <p className="auth-card__eyebrow">RECUPERE O ACESSO</p>
        <h1 id="forgot-password-title">Esqueceu sua senha?</h1>
        <p className="auth-card__description">
          Informe o e-mail da sua conta. Se ele estiver cadastrado, enviaremos
          um link para criar uma nova senha.
        </p>

        <form className="auth-form" onSubmit={handleSubmit}>
          <label htmlFor="forgot-password-email">E-mail</label>
          <input
            id="forgot-password-email"
            type="email"
            autoComplete="email"
            maxLength={320}
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            required
          />
          {error && <p className="auth-form__error" role="alert">{error}</p>}
          {message && <p className="auth-form__success" role="status">{message}</p>}
          <button type="submit" disabled={saving}>
            {saving ? "Enviando..." : "Enviar instruções"}
          </button>
        </form>

        <p className="auth-card__switch">
          Lembrou sua senha? <Link to="/login">Voltar para o login</Link>
        </p>
      </section>
    </main>
  );
}
