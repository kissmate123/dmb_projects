const API_URL = "/api";

async function readError(res) {
  const ct = res.headers.get("content-type") || "";

  if (ct.includes("application/json")) {
    const j = await res.json().catch(() => null);

    if (j?.message) return j.message;

    if (Array.isArray(j?.errors)) {
      return j.errors.join(" ");
    }

    if (j?.errors && typeof j.errors === "object") {
      return Object.values(j.errors).flat().join(" ");
    }

    return JSON.stringify(j);
  }

  return (await res.text().catch(() => "")) || "Hiba történt.";
}

export async function getNews() {
  const res = await fetch(`${API_URL}/News`, {
    method: "GET",
    headers: { Accept: "application/json" },
  });

  if (!res.ok) {
    throw new Error(await readError(res));
  }

  return res.json();
}

export async function createNews(token, title, text) {
  const res = await fetch(`${API_URL}/News`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ title, text }),
  });

  if (!res.ok) {
    throw new Error(await readError(res));
  }

  return res.json();
}

export async function deleteNews(token, id) {
  const res = await fetch(`${API_URL}/News/${id}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!res.ok) {
    throw new Error(await readError(res));
  }
}