import React, { createContext, useContext, useEffect, useState } from "react";

const AuthContext = createContext(null);

function base64UrlDecode(str) {
  const base64 = str.replace(/-/g, "+").replace(/_/g, "/");
  const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), "=");

  try {
    return decodeURIComponent(
      atob(padded)
        .split("")
        .map((c) => "%" + c.charCodeAt(0).toString(16).padStart(2, "0"))
        .join("")
    );
  } catch {
    return atob(padded);
  }
}

function decodeJwt(token) {
  if (!token) return null;
  const parts = token.split(".");
  if (parts.length !== 3) return null;

  try {
    const payloadJson = base64UrlDecode(parts[1]);
    return JSON.parse(payloadJson);
  } catch {
    return null;
  }
}

function extractUserName(payload) {
  if (!payload) return "";

  if (payload.unique_name) return payload.unique_name;

  const nameClaimKey = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
  if (payload[nameClaimKey]) return payload[nameClaimKey];

  if (payload.name) return payload.name;

  return "";
}

function extractIsAdmin(payload) {
  if (!payload) return false;

  if (payload.is_admin === true) return true;
  if (typeof payload.is_admin === "string" && payload.is_admin.toLowerCase() === "true") return true;

  return false;
}

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => localStorage.getItem("token") || "");
  const [userName, setUserName] = useState("");
  const [isAdmin, setIsAdmin] = useState(false);

  useEffect(() => {
    if (!token) {
      setUserName("");
      setIsAdmin(false);
      return;
    }

    const payload = decodeJwt(token);
    setUserName(extractUserName(payload));
    setIsAdmin(extractIsAdmin(payload));
  }, [token]);

  const login = (newToken) => {
    localStorage.setItem("token", newToken);
    setToken(newToken);
  };

  const logout = () => {
    localStorage.removeItem("token");
    setToken("");
    setUserName("");
    setIsAdmin(false);
  };

  return (
    <AuthContext.Provider value={{ token, userName, isAdmin, isLoggedIn: !!token, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside AuthProvider");
  return ctx;
}
