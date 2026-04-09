import React, { createContext, useCallback, useContext, useEffect, useState } from "react";

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

  const val = payload.is_admin;

  if (val === true) return true;
  if (val === false) return false;

  if (typeof val === "string") return val.toLowerCase() === "true";

  return false;
}

function isTokenExpired(payload) {
  if (!payload?.exp) return false;
  const nowInSeconds = Math.floor(Date.now() / 1000);
  return payload.exp <= nowInSeconds;
}

function getValidStoredToken() {
  const stored = localStorage.getItem("token") || "";
  if (!stored) return "";

  const payload = decodeJwt(stored);
  if (!payload || isTokenExpired(payload)) {
    localStorage.removeItem("token");
    return "";
  }

  return stored;
}

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => getValidStoredToken());

  const [userName, setUserName] = useState(() => {
    const t = getValidStoredToken();
    const payload = decodeJwt(t);
    return extractUserName(payload);
  });

  const [isAdmin, setIsAdmin] = useState(() => {
    const t = getValidStoredToken();
    const payload = decodeJwt(t);
    return extractIsAdmin(payload);
  });

  const logout = useCallback(() => {
    localStorage.removeItem("token");
    setToken("");
    setUserName("");
    setIsAdmin(false);
  }, []);

  const login = useCallback((newToken) => {
    const payload = decodeJwt(newToken);

    if (!payload || isTokenExpired(payload)) {
      logout();
      throw new Error("Érvénytelen vagy lejárt token.");
    }

    localStorage.setItem("token", newToken);
    setToken(newToken);
  }, [logout]);

  useEffect(() => {
    if (!token) {
      setUserName("");
      setIsAdmin(false);
      return;
    }

    const payload = decodeJwt(token);

    if (!payload || isTokenExpired(payload)) {
      logout();
      return;
    }

    setUserName(extractUserName(payload));
    setIsAdmin(extractIsAdmin(payload));

    let timeoutId;

    if (payload.exp) {
      const msUntilExpiry = payload.exp * 1000 - Date.now();

      if (msUntilExpiry <= 0) {
        logout();
        return;
      }

      timeoutId = setTimeout(() => {
        logout();
      }, msUntilExpiry);
    }

    return () => {
      if (timeoutId) clearTimeout(timeoutId);
    };
  }, [token, logout]);

  return (
    <AuthContext.Provider
      value={{
        token,
        userName,
        isAdmin,
        login,
        logout,
        isLoggedIn: !!token,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside AuthProvider");
  return ctx;
}