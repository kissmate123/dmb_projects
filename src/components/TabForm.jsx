import React, { useState } from "react";

const TabForm = () => {
  const [activeTab, setActiveTab] = useState("login");

  return (
    <div className="tartalom">
      <div className="tab-buttons">
        <button
          className={activeTab === "login" ? "active" : ""}
          onClick={() => setActiveTab("login")}
        >
          Bejelentkezés
        </button>

        <button
          className={activeTab === "register" ? "active" : ""}
          onClick={() => setActiveTab("register")}
        >
          Regisztráció
        </button>
      </div>

      {activeTab === "login" && (
        <div className="form-content active">
          <h2>Bejelentkezés</h2>
          <form method="post" action="login.php">
            <label>Felhasználónév</label>
            <input type="text" name="Uname" placeholder="Felhasználónév" />
            <label>Jelszó</label>
            <input type="password" name="pass" placeholder="Jelszó" />
            <input className="button" type="submit" value="Bejelentkezés" />
          </form>
        </div>
      )}

      {activeTab === "register" && (
        <div className="form-content active">
          <h2>Regisztráció</h2>
          <form method="post" action="register.php">
            <label>Felhasználónév</label>
            <input type="text" name="Uname" placeholder="Felhasználónév" />
            <label>Jelszó</label>
            <input type="password" name="pass" placeholder="Jelszó" />
            <input className="button" type="submit" value="Regisztráció" />
          </form>
        </div>
      )}
    </div>
  );
};

export default TabForm;
