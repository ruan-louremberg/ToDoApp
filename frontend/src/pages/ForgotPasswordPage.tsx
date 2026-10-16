import { useEffect, useState, type FormEvent } from "react";
import { Link } from "react-router-dom";
import { requestPasswordReset } from "../api/auth";

export function ForgotPasswordPage() {
  const [email, setEmail] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [cooldownSeconds, setCooldownSeconds] = useState(0);

  useEffect(() => {
    if (cooldownSeconds === 0) {
      return;
    }

    const timer = window.setTimeout(() => {
      setCooldownSeconds((seconds) => Math.max(0, seconds - 1));
    }, 1000);

    return () => window.clearTimeout(timer);
  }, [cooldownSeconds]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSaving(true);
    setError(null);
    setMessage(null);

    try {
      const response = await requestPasswordReset({ email });
      setMessage(response.message);
      setCooldownSeconds(60);
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
          {cooldownSeconds > 0 && (
            <span className="auth-form__hint" role="timer" aria-live="off">
              Você poderá solicitar outro link em {Math.floor(cooldownSeconds / 60)}:
              {String(cooldownSeconds % 60).padStart(2, "0")}.
            </span>
          )}
          <button type="submit" disabled={saving || cooldownSeconds > 0}>
            {saving
              ? "Enviando..."
              : cooldownSeconds > 0
                ? `Aguarde ${Math.floor(cooldownSeconds / 60)}:${String(cooldownSeconds % 60).padStart(2, "0")}`
                : "Enviar instruções"}
          </button>
        </form>

        <p className="auth-card__switch">
          Lembrou sua senha? <Link to="/login">Voltar para o login</Link>
        </p>
      </section>
    </main>
  );
}
