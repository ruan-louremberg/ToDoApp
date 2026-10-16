import { useState, type FormEvent } from "react";
import { Eye, EyeOff } from "lucide-react";
import { Link, useSearchParams } from "react-router-dom";
import { resetPassword } from "../api/auth";

export function ResetPasswordPage() {
  const [searchParams] = useSearchParams();
  const token = searchParams.get("token") ?? "";
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);
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
      await resetPassword({ token, password });
      setSuccess(true);
    } catch (requestError) {
      setError(
        requestError instanceof Error
          ? requestError.message
          : "Não foi possível redefinir sua senha."
      );
    } finally {
      setSaving(false);
    }
  }

  return (
    <main className="auth-page">
      <section className="auth-card" aria-labelledby="reset-password-title">
        <Link className="auth-brand" to="/login" aria-label="todo.">
          <span className="header__brand-mark" aria-hidden="true">T</span>
          <span>todo<span className="header__brand-accent">.</span></span>
        </Link>
        <p className="auth-card__eyebrow">NOVA SENHA</p>
        <h1 id="reset-password-title">Redefina sua senha</h1>

        {success ? (
          <>
            <p className="auth-form__success" role="status">
              Sua senha foi alterada. Agora você já pode entrar com a nova senha.
            </p>
            <p className="auth-card__switch">
              <Link to="/login">Ir para o login</Link>
            </p>
          </>
        ) : (
          <>
            <p className="auth-card__description">
              Escolha uma senha com pelo menos 8 caracteres.
            </p>
            {!token && (
              <p className="auth-form__error" role="alert">
                O link está incompleto. Solicite uma nova redefinição de senha.
              </p>
            )}
            <form className="auth-form" onSubmit={handleSubmit}>
              <label htmlFor="reset-password">Nova senha</label>
              <div className="auth-password-field">
                <input
                  id="reset-password"
                  type={showPassword ? "text" : "password"}
                  autoComplete="new-password"
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
              <label htmlFor="reset-password-confirm">Confirme a nova senha</label>
              <div className="auth-password-field">
                <input
                  id="reset-password-confirm"
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
              <button type="submit" disabled={saving || !token}>
                {saving ? "Salvando..." : "Redefinir senha"}
              </button>
            </form>
            <p className="auth-card__switch">
              <Link to="/forgot-password">Solicitar outro link</Link>
            </p>
          </>
        )}
      </section>
    </main>
  );
}
