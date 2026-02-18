import React, { useEffect, useMemo, useState } from "react";
import { useAuth } from "../context/AuthContext";

const STORAGE_KEY = "pixelrealms_news_v1";

function safeParse(json) {
  try {
    const x = JSON.parse(json);
    return Array.isArray(x) ? x : [];
  } catch {
    return [];
  }
}

function makeId() {
  if (typeof crypto !== "undefined" && crypto.randomUUID) return crypto.randomUUID();
  return String(Date.now()) + "_" + Math.random().toString(16).slice(2);
}

function formatDate(iso) {
  try {
    const d = new Date(iso);
    return d.toLocaleDateString("hu-HU", { year: "numeric", month: "long", day: "numeric" });
  } catch {
    return "";
  }
}

export default function NewsSection() {
  const { isAdmin } = useAuth();

  const [items, setItems] = useState(() => {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? safeParse(raw) : [];
  });

  // form state (admin)
  const [title, setTitle] = useState("");
  const [excerpt, setExcerpt] = useState("");

  useEffect(() => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
  }, [items]);

  const visibleItems = useMemo(() => {
    // már eleve 4-re vágunk mentéskor, de biztos ami biztos:
    return [...items]
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 4);
  }, [items]);

  const canAdd = isAdmin;

  const handleAdd = (e) => {
    e.preventDefault();
    if (!canAdd) return;

    const trimmedTitle = title.trim();
    const trimmedExcerpt = excerpt.trim();

    // ha üres, ne engedjük felvenni
    if (!trimmedTitle || !trimmedExcerpt) return;

    const newItem = {
      id: makeId(),
      title: trimmedTitle,
      excerpt: trimmedExcerpt,
      createdAt: new Date().toISOString(),
    };

    const next = [newItem, ...items]
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 4); // max 4, a legrégebbi kiesik

    setItems(next);
    setTitle("");
    setExcerpt("");
  };

  return (
    <section className="news">
      <div className="news-header">
        <h2 className="news-title">NEWS</h2>

        {canAdd ? (
          <span className="news-admin-badge">Admin</span>
        ) : null}
      </div>

      {canAdd && (
        <form className="news-admin" onSubmit={handleAdd}>
          <div className="news-admin-row">
            <label className="news-admin-label">Cím</label>
            <input
              className="news-admin-input"
              type="text"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder=""
            />
          </div>

          <div className="news-admin-row">
            <label className="news-admin-label">Leírás</label>
            <textarea
              className="news-admin-textarea"
              value={excerpt}
              onChange={(e) => setExcerpt(e.target.value)}
              rows={3}
              placeholder=""
            />
          </div>

          <div className="news-admin-actions">
            <button className="news-admin-btn" type="submit" disabled={!title.trim() || !excerpt.trim()}>
              Hír hozzáadása
            </button>
            <div className="news-admin-note">Maximum 4 hír látszik, az új felülírja a legrégebbit.</div>
          </div>
        </form>
      )}

      <div className="news-grid">
        {visibleItems.length === 0 ? (
          <div className="news-empty">Nincs hír.</div>
        ) : (
          visibleItems.map((n, idx) => (
            <article className="news-card" key={n.id}>
              <div
                className="news-card-accent"
                style={{ background: idx === 3 ? "#ff2f86" : "#a855f7" }}
              />
              <div className="news-card-body">
                <div className="news-date">{formatDate(n.createdAt)}</div>
                <h3 className="news-headline">{n.title}</h3>
                <p className="news-excerpt">{n.excerpt}</p>
                <div className="news-link"> </div>
              </div>
            </article>
          ))
        )}
      </div>
    </section>
  );
}
