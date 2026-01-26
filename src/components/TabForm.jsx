import React, { useState } from "react";

const API_URL = "https://localhost:7273"; // <-- IDE a backend címed (Swaggerből)

const TabForm = () => {
  const [activeTab, setActiveTab] = useState("login");

  // Login mezők
  const [loginEmail, setLoginEmail] = useState("");
  const [loginPassword, setLoginPassword] = useState("");

  // Register mezők
  const [regEmail, setRegEmail] = useState("");
  const [regPassword, setRegPassword] = useState("");

  // Üzenet (hiba / siker)
  const [message, setMessage] = useState("");

  // ✅ LOGIN
  const handleLogin = async (e) => {
    e.preventDefault();
    setMessage("");

    try {
      const res = await fetch(`${API_URL}/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          email: loginEmail,
          password: loginPassword,
        }),
      });

      if (!res.ok) {
        const err = await res.text();
        throw new Error(err);
      }

      const data = await res.json(); // { token, expiresAt }
      localStorage.setItem("token", data.token);

      setMessage("Sikeres bejelentkezés ✅");
    } catch (err) {
      setMessage(err.message);
    }
  };

  // ✅ REGISTER
  const handleRegister = async (e) => {
    e.preventDefault();
    setMessage("");

    try {
      const res = await fetch(`${API_URL}/api/auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          email: regEmail,
          password: regPassword,
        }),
      });

      if (!res.ok) {
        const err = await res.text();
        throw new Error(err);
      }

      setMessage("Sikeres regisztráció ✅ Most jelentkezz be!");
      setActiveTab("login");

      // ürítjük a mezőket
      setRegEmail("");
      setRegPassword("");
    } catch (err) {
      setMessage(err.message);
    }
  };

  return (
    <div className="login_reg_tab tartalom1">
      <p></p>

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

      {/* Üzenet kiírás */}
      {message && (
        <p style={{ marginTop: "10px", fontWeight: "bold" }}>{message}</p>
      )}

      {/* LOGIN TAB */}
      {activeTab === "login" && (
        <div className="form-content active">
          <h2>Bejelentkezés</h2>

          <form onSubmit={handleLogin}>
            <label>Email</label>
            <input
              type="text"
              placeholder="Email"
              value={loginEmail}
              onChange={(e) => setLoginEmail(e.target.value)}
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

      {/* REGISTER TAB */}
      {activeTab === "register" && (
        <div className="form-content active">
          <h2>Regisztráció</h2>

          <form onSubmit={handleRegister}>
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

            <input className="button" type="submit" value="Regisztráció" />
          </form>
        </div>
      )}
    </div>
  );
};

export default TabForm;
