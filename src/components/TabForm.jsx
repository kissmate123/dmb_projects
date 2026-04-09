import React, { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const API_URL = "/api";

async function readResponseError(res) {
  const ct = res.headers.get("content-type") || "";

  if (ct.includes("application/json")) {
    const errJson = await res.json().catch(() => null);

    if (errJson?.message) return errJson.message;
    if (Array.isArray(errJson?.errors)) return errJson.errors.join(" ");
    if (errJson?.errors && typeof errJson.errors === "object") {
      return Object.values(errJson.errors).flat().join(" ");
    }

    return "Hiba történt!";
  }

  const text = await res.text().catch(() => "");
  return text || "Hiba történt!";
}

const TabForm = () => {
  const [activeTab, setActiveTab] = useState("login");

  const [loginIdentifier, setLoginIdentifier] = useState("");
  const [loginPassword, setLoginPassword] = useState("");

  const [regUserName, setRegUserName] = useState("");
  const [regEmail, setRegEmail] = useState("");
  const [regPassword, setRegPassword] = useState("");

  const [message, setMessage] = useState("");

  const navigate = useNavigate();
  const auth = useAuth();

  const passwordRules = useMemo(() => {
    const p = regPassword || "";

    const lengthOk = p.length >= 8;
    const uppercaseOk = /[A-Z]/.test(p);
    const digitOk = /\d/.test(p);
    const specialOk = /[^A-Za-z0-9]/.test(p);

    return {
      lengthOk,
      uppercaseOk,
      digitOk,
      specialOk,
      allOk: lengthOk && uppercaseOk && digitOk && specialOk,
    };
  }, [regPassword]);

  const handleLogin = async (e) => {
    e.preventDefault();
    setMessage("");

    try {
      const res = await fetch(`${API_URL}/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          identifier: loginIdentifier.trim(),
          password: loginPassword,
        }),
      });

      if (!res.ok) {
        throw new Error(await readResponseError(res));
      }

      const data = await res.json();
      auth.login(data.token);

      setLoginIdentifier("");
      setLoginPassword("");
      navigate("/", { replace: true });
    } catch (err) {
      setMessage(err.message || "Sikertelen bejelentkezés.");
    }
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    setMessage("");

    if (!passwordRules.allOk) {
      setMessage("A jelszó nem felel meg a követelményeknek.");
      return;
    }

    try {
      const res = await fetch(`${API_URL}/auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          email: regEmail.trim(),
          userName: regUserName.trim(),
          password: regPassword,
        }),
      });

      if (!res.ok) {
        throw new Error(await readResponseError(res));
      }

      const ct = res.headers.get("content-type") || "";
      if (ct.includes("application/json")) {
        const okJson = await res.json().catch(() => null);
        setMessage(okJson?.message || "Sikeres regisztráció. Most jelentkezz be!");
      } else {
        const okText = await res.text().catch(() => "");
        setMessage(okText || "Sikeres regisztráció. Most jelentkezz be!");
      }

      setActiveTab("login");
      setRegUserName("");
      setRegEmail("");
      setRegPassword("");
    } catch (err) {
      setMessage(err.message || "Sikertelen regisztráció.");
    }
  };

  return (
    <div className="login_reg_tab tartalom1">
      <div className="tab-buttons">
        <button
          className={activeTab === "login" ? "active" : ""}
          onClick={() => setActiveTab("login")}
          type="button"
        >
          Bejelentkezés
        </button>

        <button
          className={activeTab === "register" ? "active" : ""}
          onClick={() => setActiveTab("register")}
          type="button"
        >
          Regisztráció
        </button>
      </div>

      {message && (
        <p style={{ marginTop: "10px", fontWeight: "bold" }}>{message}</p>
      )}

      {activeTab === "login" && (
        <div className="form-content active">
          <h2>Bejelentkezés</h2>

          <form onSubmit={handleLogin}>
            <label>Email vagy Felhasználónév</label>
            <input
              type="text"
              placeholder="Email vagy Felhasználónév"
              value={loginIdentifier}
              onChange={(e) => setLoginIdentifier(e.target.value)}
            />

            <label>Jelszó</label>
            <input
              type="password"
              placeholder="Jelszó"
              value={loginPassword}
              onChange={(e) => setLoginPassword(e.target.value)}
            />

            <input className="button" type="submit" value="Bejelentkezés" />
          </form>
        </div>
      )}

      {activeTab === "register" && (
        <div className="form-content active">
          <h2>Regisztráció</h2>

          <form onSubmit={handleRegister}>
            <label>Felhasználónév</label>
            <input
              type="text"
              placeholder="Felhasználónév"
              value={regUserName}
              onChange={(e) => setRegUserName(e.target.value)}
            />

            <label>Email</label>
            <input
              type="text"
              placeholder="Email"
              value={regEmail}
              onChange={(e) => setRegEmail(e.target.value)}
            />

            <label>Jelszó</label>
            <input
              type="password"
              placeholder="Jelszó"
              value={regPassword}
              onChange={(e) => setRegPassword(e.target.value)}
            />

            <div className="password-hints">
              <div className={passwordRules.allOk ? "pw-status ok" : "pw-status bad"}>
                {passwordRules.allOk
                  ? "A jelszó megfelel a követelményeknek."
                  : "A jelszónak meg kell felelnie az alábbi feltételeknek:"}
              </div>

              <ul className="pw-rules">
                <li className={passwordRules.lengthOk ? "ok" : "bad"}>
                  Legalább 8 karakter
                </li>
                <li className={passwordRules.uppercaseOk ? "ok" : "bad"}>
                  Legalább 1 nagybetű
                </li>
                <li className={passwordRules.digitOk ? "ok" : "bad"}>
                  Legalább 1 szám
                </li>
                <li className={passwordRules.specialOk ? "ok" : "bad"}>
                  Legalább 1 speciális karakter
                </li>
              </ul>
            </div>

            <input
              className="button"
              type="submit"
              value="Regisztráció"
              disabled={!passwordRules.allOk}
            />
          </form>
        </div>
      )}
    </div>
  );
};

export default TabForm;