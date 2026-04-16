import React, { useEffect, useRef, useState } from "react";
import { Link, NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const Navbar = () => {
  const { userName, isLoggedIn, isAdmin, logout } = useAuth();
  const [open, setOpen] = useState(false);
  const [scrolled, setScrolled] = useState(false);
  const menuRef = useRef(null);
  const navigate = useNavigate();

  useEffect(() => {
    const onScroll = () => setScrolled(window.scrollY > 10);
    onScroll();
    window.addEventListener("scroll", onScroll);
    return () => window.removeEventListener("scroll", onScroll);
  }, []);

  useEffect(() => {
    function handleClickOutside(e) {
      if (menuRef.current && !menuRef.current.contains(e.target)) {
        setOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const handleLogout = () => {
    logout();
    setOpen(false);
    navigate("/", { replace: true });
  };

  const handleAdminClick = () => {
  const link = document.createElement("a");
  link.href = "/assets/apps/mysetup.exe";
  link.setAttribute("download", "mysetup.exe");
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);

  setOpen(false);
};
  return (
    <nav className={`navbar ${scrolled ? "navbar--scrolled" : ""}`}>
      <div className="nav-container">
        <Link className="nav-logo" to="/">
          <img
            src="/assets/images/logo_favi.png"
            alt="Logo"
            className="logo-image"
            style={{ width: "40px", marginRight: "8px", borderRadius: "5px" }}
          />
        </Link>

        <ul className="nav-links">
          <li>
            <NavLink to="/">Otthon</NavLink>
          </li>
          <li>
            <NavLink to="/karakterek">Karakterek</NavLink>
          </li>

          <li style={{ position: "relative" }} ref={menuRef}>
            {!isLoggedIn ? (
              <NavLink to="/login">Bejelentkezés</NavLink>
            ) : (
              <>
                <button
                  type="button"
                  onClick={() => setOpen((v) => !v)}
                  style={{
                    background: "transparent",
                    border: "none",
                    color: "inherit",
                    cursor: "pointer",
                    font: "inherit",
                    padding: 0,
                  }}
                >
                  {userName || "Profil"}
                </button>

                {open && (
                  <div
                    style={{
                      position: "absolute",
                      right: 0,
                      top: "calc(100% + 10px)",
                      minWidth: "180px",
                      background: "rgba(0,0,0,0.85)",
                      border: "1px solid rgba(255,255,255,0.12)",
                      borderRadius: "10px",
                      padding: "8px",
                      zIndex: 9999,
                    }}
                  >
                    {isAdmin && (
                      <button
                        type="button"
                        onClick={handleAdminClick}
                        style={{
                          width: "100%",
                          background: "transparent",
                          border: "none",
                          color: "rgba(255,255,255,0.6)",
                          textAlign: "left",
                          padding: "10px",
                          cursor: "pointer",
                        }}
                        title="Admin applikáció"
                      >
                        Admin felület
                      </button>
                    )}

                    <button
                      type="button"
                      onClick={handleLogout}
                      style={{
                        width: "100%",
                        background: "transparent",
                        border: "none",
                        color: "inherit",
                        textAlign: "left",
                        padding: "10px",
                        cursor: "pointer",
                      }}
                    >
                      Kijelentkezés
                    </button>
                  </div>
                )}
              </>
            )}
          </li>
        </ul>
      </div>
    </nav>
  );
};

export default Navbar;
