import React, { useEffect, useMemo, useState } from "react";

const STORAGE_KEY = "pixelrealms_news_items";
const MAX_VISIBLE = 4;

const ACCENT_COLORS = ["#c77dff", "#a855f7", "#ff0000", "#ff4dd2"];

function decodeJwtPayload(token) {
  try {
    const parts = token.split(".");
    if (parts.length < 2) return null;

    const base64 = parts[1].replace(/-/g, "+").replace(/_/g, "/");
    const padded = base64 + "=".repeat((4 - (base64.length % 4)) % 4);
    const json = decodeURIComponent(
      atob(padded)
        .split("")
        .map((c) => "%" + c.charCodeAt(0).toString(16).padStart(2, "0"))
        .join("")
    );

    return JSON.parse(json);
  } catch {
    return null;
  }
}

function getStoredToken() {
  // igazítsd, ha nálad más kulcs alatt van
  const direct =
    localStorage.getItem("token") ||
    localStorage.getItem("authToken") ||
    localStorage.getItem("jwt");

  if (direct) return direct;

  // ha objektumban tárolod
  const authRaw = localStorage.getItem("auth");
  if (authRaw) {
    try {
      const parsed = JSON.parse(authRaw);
      if (parsed?.token) return parsed.token;
    } catch {}
  }

  const userRaw = localStorage.getItem("user");
  if (userRaw) {
    try {
      const parsed = JSON.parse(userRaw);
      if (parsed?.token) return parsed.token;
    } catch {}
  }

  return null;
}

function isAdminUser() {
  const token = getStoredToken();
  if (!token) return false;

  const payload = decodeJwtPayload(token);
  if (!payload) return false;

  const isAdminClaim =
    payload.is_admin === true ||
    payload.is_admin === "true" ||
    payload["is_admin"] === true ||
    payload["is_admin"] === "true";

  const email =
    payload.email ||
    payload[
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
    ] ||
    payload[
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
    ];

  const userName =
    payload.unique_name ||
    payload[
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
    ] ||
    payload.name;

  const adminByIdentity =
    String(email || "").toLowerCase() === "admin44@gmail.com" ||
    String(userName || "").toLowerCase() === "admin";

  return Boolean(isAdminClaim || adminByIdentity);
}

function formatDateHu(dateLike) {
  const d = new Date(dateLike);
  if (Number.isNaN(d.getTime())) return "";

  return d.toLocaleDateString("hu-HU", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
}

function normalizeNewsItem(item) {
  return {
    id: item.id ?? crypto.randomUUID(),
    title: String(item.title ?? "").trim(),
    excerpt: String(item.excerpt ?? "").trim(),
    createdAt: item.createdAt ?? new Date().toISOString(),
  };
}

export default function NewsSection() {
  const [isAdmin, setIsAdmin] = useState(false);
  const [newsItems, setNewsItems] = useState([]);
  const [title, setTitle] = useState("");
  const [excerpt, setExcerpt] = useState("");

  // admin státusz frissítése betöltéskor + storage változáskor
  useEffect(() => {
    const refreshAdmin = () => setIsAdmin(isAdminUser());

    refreshAdmin();
    window.addEventListener("storage", refreshAdmin);

    return () => window.removeEventListener("storage", refreshAdmin);
  }, []);

  // hírek betöltése
  useEffect(() => {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (!raw) {
        setNewsItems([]);
        return;
      }

      const parsed = JSON.parse(raw);
      if (!Array.isArray(parsed)) {
        setNewsItems([]);
        return;
      }

      const normalized = parsed
        .map(normalizeNewsItem)
        .filter((x) => x.title || x.excerpt);

      setNewsItems(normalized);
    } catch {
      setNewsItems([]);
    }
  }, []);

  // mentés localStorage-ba
  useEffect(() => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(newsItems));
  }, [newsItems]);

  // legújabbak elöl
  const sortedNews = useMemo(() => {
    return [...newsItems].sort(
      (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
    );
  }, [newsItems]);

  // csak 4 látszik
  const visibleNews = sortedNews.slice(0, MAX_VISIBLE);

  const handleAddNews = (e) => {
    e.preventDefault();
    if (!isAdmin) return;

    const t = title.trim();
    const ex = excerpt.trim();

    if (!t || !ex) return;

    const newItem = {
      id: crypto.randomUUID(),
      title: t,
      excerpt: ex,
      createdAt: new Date().toISOString(),
    };

    setNewsItems((prev) => [newItem, ...prev]); // több is maradhat, csak 4 látszik
    setTitle("");
    setExcerpt("");
  };

  const handleDeleteNews = (id) => {
    if (!isAdmin) return;

    const ok = window.confirm("Biztosan törölni szeretnéd ezt a hírt?");
    if (!ok) return;

    setNewsItems((prev) => prev.filter((item) => item.id !== id));
  };

  return (
    <div className="news">
      <div className="news-header">
        <h2 className="title">News</h2>
        {isAdmin && <span className="news-admin-badge">Admin mód</span>}
      </div>

      {isAdmin && (
        <form className="news-admin" onSubmit={handleAddNews}>
          <div className="news-admin-row">
            <label className="news-admin-label" htmlFor="news-title">
              Cím
            </label>
            <input
              id="news-title"
              className="news-admin-input"
              type="text"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder=""
              maxLength={120}
            />
          </div>

          <div className="news-admin-row">
            <label className="news-admin-label" htmlFor="news-excerpt">
              Leírás
            </label>
            <textarea
              id="news-excerpt"
              className="news-admin-textarea"
              value={excerpt}
              onChange={(e) => setExcerpt(e.target.value)}
              placeholder="Rövid leírás..."
              maxLength={500}
            />
          </div>

          <div className="news-admin-actions">
            <button
              className="news-admin-btn"
              type="submit"
              disabled={!title.trim() || !excerpt.trim()}
            >
              Hír hozzáadása
            </button>

            <span className="news-admin-note">
              Maximum 4 hír látszik egyszerre (a legrégebbi rejtve marad).
            </span>
          </div>
        </form>
      )}

      {visibleNews.length === 0 ? (
        <div className="news-empty">
          Még nincs hír hozzáadva.
        </div>
      ) : (
        <div className="news-grid">
          {visibleNews.map((item, index) => (
            <article className="news-card" key={item.id}>
              <div
                className="news-card-accent"
                style={{ background: ACCENT_COLORS[index % ACCENT_COLORS.length] }}
              />

              {isAdmin && (
                <button
                  type="button"
                  className="news-delete-btn"
                  onClick={() => handleDeleteNews(item.id)}
                  aria-label="Hír törlése"
                  title="Törlés"
                >
                  ×
                </button>
              )}

              <div className="news-card-body">
                <div className="news-date">{formatDateHu(item.createdAt)}</div>

                <h3 className="news-headline">{item.title}</h3>

                <p className="news-excerpt">{item.excerpt}</p>

                <div className="news-link" />
              </div>
            </article>
          ))}
        </div>
      )}
    </div>
  );
}