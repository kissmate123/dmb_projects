import React, { useEffect, useMemo, useRef, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { getNews, createNews, deleteNews } from "../services/newsService";

function formatHuDate(d) {
  try {
    const date = new Date(d);
    if (Number.isNaN(date.getTime())) return "";
    return date.toLocaleDateString("hu-HU", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  } catch {
    return "";
  }
}

export default function NewsSection() {
  const { token, isAdmin } = useAuth();

  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);

  const [title, setTitle] = useState("");
  const [text, setText] = useState("");

  const [error, setError] = useState("");
  const [saving, setSaving] = useState(false);

  const [expandedItem, setExpandedItem] = useState(null);
  const [overflowMap, setOverflowMap] = useState({});

  const excerptRefs = useRef({});

  const visibleItems = useMemo(() => {
    const arr = Array.isArray(items) ? [...items] : [];

    arr.sort((a, b) => {
      const da = new Date(
        a.createdAtUtc ?? a.CreatedAtUtc ?? a.createdAt ?? a.CreatedAt ?? a.date ?? a.Date ?? 0
      ).getTime();

      const db = new Date(
        b.createdAtUtc ?? b.CreatedAtUtc ?? b.createdAt ?? b.CreatedAt ?? b.date ?? b.Date ?? 0
      ).getTime();

      return db - da;
    });

    return arr.slice(0, 4);
  }, [items]);

  function measureOverflow() {
    const next = {};

    visibleItems.forEach((x) => {
      const id = x.id ?? x.Id;
      const el = excerptRefs.current[id];

      if (el) {
        next[id] = el.scrollHeight > el.clientHeight + 1;
      }
    });

    setOverflowMap(next);
  }

  async function refresh() {
    setError("");
    setLoading(true);

    try {
      const data = await getNews();
      setItems(Array.isArray(data) ? data : data?.items ?? []);
    } catch (e) {
      console.error("GET /api/News hiba:", e);
      setError(e?.message || "Nem sikerült betölteni a híreket.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    refresh();
  }, []);

  useEffect(() => {
    const raf = requestAnimationFrame(() => {
      measureOverflow();
    });

    const onResize = () => measureOverflow();
    window.addEventListener("resize", onResize);

    return () => {
      cancelAnimationFrame(raf);
      window.removeEventListener("resize", onResize);
    };
  }, [visibleItems, loading]);

  useEffect(() => {
    function onKeyDown(e) {
      if (e.key === "Escape") {
        setExpandedItem(null);
      }
    }

    window.addEventListener("keydown", onKeyDown);
    return () => window.removeEventListener("keydown", onKeyDown);
  }, []);

  async function onAdd(e) {
    e.preventDefault();
    setError("");

    if (!isAdmin) {
      setError("Ehhez admin jogosultság szükséges.");
      return;
    }

    if (!token) {
      setError("Hiányzik a bejelentkezési token.");
      return;
    }

    if (!title.trim() || !text.trim()) {
      setError("A cím és a szöveg megadása kötelező.");
      return;
    }

    try {
      setSaving(true);
      await createNews(token, title.trim(), text.trim());
      setTitle("");
      setText("");
      await refresh();
    } catch (e) {
      console.error("POST /api/News hiba:", e);
      setError(e?.message || "Nem sikerült létrehozni a hírt.");
    } finally {
      setSaving(false);
    }
  }

  async function onDelete(id) {
    setError("");

    if (!isAdmin) {
      setError("Ehhez admin jogosultság szükséges.");
      return;
    }

    if (!token) {
      setError("Hiányzik a bejelentkezési token.");
      return;
    }

    try {
      await deleteNews(token, id);
      setItems((prev) => prev.filter((x) => (x.id ?? x.Id) !== id));

      setExpandedItem((prev) => {
        const prevId = prev?.id ?? prev?.Id;
        return prevId === id ? null : prev;
      });
    } catch (e) {
      console.error("DELETE /api/News/{id} hiba:", e);
      setError(e?.message || "Nem sikerült törölni a hírt.");
    }
  }

  return (
    <>
      <div className="news">
        <div className="news-header">
          <h2 className="title">FRISSÍTÉSEK</h2>
          {isAdmin ? <div className="news-admin-badge">ADMIN</div> : null}
        </div>

        {error ? (
          <p style={{ margin: "0 20px 14px", fontFamily: "Segoe UI, Arial, sans-serif" }}>
            {error}
          </p>
        ) : null}

        {isAdmin ? (
          <form className="news-admin" onSubmit={onAdd}>
            <div className="news-admin-row">
              <div className="news-admin-label">Cím</div>
              <input
                className="news-admin-input"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                placeholder="Cím"
                type="text"
              />
            </div>

            <div className="news-admin-row">
              <div className="news-admin-label">Szöveg</div>
              <textarea
                className="news-admin-textarea"
                value={text}
                onChange={(e) => setText(e.target.value)}
                placeholder="Szöveg"
              />
            </div>

            <div className="news-admin-actions">
              <button
                className="news-admin-btn"
                type="submit"
                disabled={saving || !title.trim() || !text.trim()}
              >
                {saving ? "Mentés..." : "Hozzáadás"}
              </button>
              <div className="news-admin-note">Max. 4 kártya látszik.</div>
            </div>
          </form>
        ) : null}

        {loading ? (
          <div className="news-empty">Betöltés...</div>
        ) : visibleItems.length === 0 ? (
          <div className="news-empty">Nincs még frissítés.</div>
        ) : (
          <div className="news-grid">
            {visibleItems.map((x) => {
              const id = x.id ?? x.Id;
              const created =
                x.createdAtUtc ?? x.CreatedAtUtc ?? x.createdAt ?? x.CreatedAt ?? x.date ?? x.Date;
              const headline = x.title ?? x.Title ?? "";
              const excerpt = x.text ?? x.Text ?? x.content ?? x.Content ?? "";

              return (
                <article className="news-card" key={id}>
                  <div className="news-card-accent" style={{ background: "#a855f7" }} />

                  {isAdmin ? (
                    <button
                      type="button"
                      className="news-delete-btn"
                      onClick={() => onDelete(id)}
                      title="Törlés"
                      aria-label="Törlés"
                    >
                      ×
                    </button>
                  ) : null}

                  <div className="news-card-body">
                    <div className="news-date">{created ? formatHuDate(created) : ""}</div>

                    <h3 className="news-headline">{headline}</h3>

                    <p
                      className="news-excerpt"
                      ref={(el) => {
                        excerptRefs.current[id] = el;
                      }}
                    >
                      {excerpt}
                    </p>

                    <div className="news-card-footer">
                      {overflowMap[id] ? (
                        <button
                          type="button"
                          className="news-expand-btn"
                          onClick={() => setExpandedItem(x)}
                        >
                          Tovább
                        </button>
                      ) : null}
                    </div>
                  </div>
                </article>
              );
            })}
          </div>
        )}
      </div>

      {expandedItem ? (
        <div
          className="news-modal-overlay"
          onClick={() => setExpandedItem(null)}
        >
          <div
            className="news-modal-card"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="news-card-accent" style={{ background: "#a855f7" }} />

            <button
              type="button"
              className="news-close-btn"
              onClick={() => setExpandedItem(null)}
              title="Bezárás"
              aria-label="Bezárás"
            >
              ×
            </button>

            <div className="news-card-body-expanded">
              <div className="news-date">
                {(expandedItem.createdAtUtc ??
                  expandedItem.CreatedAtUtc ??
                  expandedItem.createdAt ??
                  expandedItem.CreatedAt ??
                  expandedItem.date ??
                  expandedItem.Date)
                  ? formatHuDate(
                      expandedItem.createdAtUtc ??
                        expandedItem.CreatedAtUtc ??
                        expandedItem.createdAt ??
                        expandedItem.CreatedAt ??
                        expandedItem.date ??
                        expandedItem.Date
                    )
                  : ""}
              </div>

              <h3 className="news-headline">
                {expandedItem.title ?? expandedItem.Title ?? ""}
              </h3>

              <div className="news-expanded-text">
                {expandedItem.text ??
                  expandedItem.Text ??
                  expandedItem.content ??
                  expandedItem.Content ??
                  ""}
              </div>
            </div>
          </div>
        </div>
      ) : null}
    </>
  );
}