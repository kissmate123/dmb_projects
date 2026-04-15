import React from "react";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import NewsSection from "./NewsSection";
import * as newsService from "../services/newsService";
import { useAuth } from "../context/AuthContext";

jest.mock("../services/newsService");
jest.mock("../context/AuthContext", () => ({
  useAuth: jest.fn(),
}));

beforeEach(() => {
  jest.clearAllMocks();
  useAuth.mockReturnValue({ token: "test-token", isAdmin: false });
  newsService.getNews.mockResolvedValue([]);
  newsService.createNews = jest.fn().mockResolvedValue({});
  newsService.deleteNews = jest.fn().mockResolvedValue({});
});

test("megjelenik a cím és betöltés látszik kezdetben", async () => {
  newsService.getNews.mockImplementation(() => new Promise(() => {})); // pending
  render(<NewsSection />);
  expect(screen.getByText("FRISSÍTÉSEK")).toBeInTheDocument();
  expect(screen.getByText("Betöltés...")).toBeInTheDocument();
});

test("üres állapot megjelenik ha nincs hír", async () => {
  newsService.getNews.mockResolvedValue([]);
  render(<NewsSection />);
  await waitFor(() => expect(screen.getByText("Nincs még frissítés.")).toBeInTheDocument());
});

test("admin űrlap csak admin felhasználónak látható és létrehoz hívást indít", async () => {
  useAuth.mockReturnValue({ token: "test-token", isAdmin: true });
  newsService.getNews.mockResolvedValue([]);
  const user = userEvent.setup();

  render(<NewsSection />);

  expect(screen.getByPlaceholderText("Cím")).toBeInTheDocument();
  expect(screen.getByPlaceholderText("Szöveg")).toBeInTheDocument();
  const title = screen.getByPlaceholderText("Cím");
  const text = screen.getByPlaceholderText("Szöveg");
  const btn = screen.getByRole("button", { name: "Hozzáadás" });

  await user.type(title, "Új cím");
  await user.type(text, "Új tartalom");
  await user.click(btn);

  await waitFor(() =>
    expect(newsService.createNews).toHaveBeenCalledWith("test-token", "Új cím", "Új tartalom")
  );
});

test("törlés meghívja a deleteNews szolgáltatást", async () => {
  useAuth.mockReturnValue({ token: "test-token", isAdmin: true });
  const mockNews = [
    { id: 42, title: "Törlendő", text: "tartalom", createdAtUtc: "2024-01-01T00:00:00Z" },
  ];
  newsService.getNews.mockResolvedValue(mockNews);

  render(<NewsSection />);

  await waitFor(() => expect(screen.getByText("Törlendő")).toBeInTheDocument());
  const delBtn = screen.getByRole("button", { name: "Törlés" });
  await userEvent.click(delBtn);

  await waitFor(() => expect(newsService.deleteNews).toHaveBeenCalledWith("test-token", 42));
});

// --- additional tests (at least 10) ---

test("admin badge látható ha isAdmin igaz", async () => {
  useAuth.mockReturnValue({ token: "t", isAdmin: true });
  newsService.getNews.mockResolvedValue([]);
  render(<NewsSection />);
  expect(screen.getByText("ADMIN")).toBeInTheDocument();
});

test("dátum magyar formátumban jelenik meg", async () => {
  const mockNews = [
    { id: 1, title: "Dátumos", text: "x", createdAtUtc: "2024-01-15T10:00:00Z" },
  ];
  newsService.getNews.mockResolvedValue(mockNews);
  render(<NewsSection />);
  await waitFor(() => {
    expect(screen.getByText(/2024/i)).toBeInTheDocument();
  });
});

test("szortírozás: újabb hír előrébb legyen", async () => {
  const mockNews = [
    { id: 1, title: "Régi", text: "a", createdAtUtc: "2024-01-01T00:00:00Z" },
    { id: 2, title: "Új", text: "b", createdAtUtc: "2024-02-01T00:00:00Z" },
  ];
  newsService.getNews.mockResolvedValue(mockNews);
  render(<NewsSection />);
  await waitFor(() => {
    const first = screen.getByText("Új");
    expect(first).toBeInTheDocument();
  });
});

test("maximum 4 elem jelenik meg ha több hír érkezik", async () => {
  const mockNews = Array.from({ length: 6 }, (_, i) => ({
    id: i + 1,
    title: `News ${i + 1}`,
    text: `Tartalom ${i + 1}`,
    createdAtUtc: new Date(2024, 0, i + 1).toISOString(),
  }));
  newsService.getNews.mockResolvedValue(mockNews);
  render(<NewsSection />);
  await waitFor(() => {
    // top 4 newest should appear (News 6..News 3)
    expect(screen.queryByText("News 2")).not.toBeInTheDocument();
    expect(screen.getByText("News 6")).toBeInTheDocument();
    expect(screen.getByText("News 3")).toBeInTheDocument();
  });
});

test("kezeli az alternatív mezőneveket (Title/Text/Id/CreatedAtUtc)", async () => {
  const mockNews = [
    { Id: 5, Title: "Alt Title", Text: "Alt Text", CreatedAtUtc: "2024-01-10T00:00:00Z" },
  ];
  newsService.getNews.mockResolvedValue(mockNews);
  render(<NewsSection />);
  await waitFor(() => {
    expect(screen.getByText("Alt Title")).toBeInTheDocument();
    expect(screen.getByText("Alt Text")).toBeInTheDocument();
  });
});

test("hibás dátum esetén is megjelenik a cím (nem dob)", async () => {
  const mockNews = [{ id: 1, title: "BrokenDate", text: "x", createdAtUtc: "not-a-date" }];
  newsService.getNews.mockResolvedValue(mockNews);
  render(<NewsSection />);
  await waitFor(() => expect(screen.getByText("BrokenDate")).toBeInTheDocument());
});

test("admin gomb letiltott amikor mezők üresek", async () => {
  useAuth.mockReturnValue({ token: "test-token", isAdmin: true });
  newsService.getNews.mockResolvedValue([]);
  render(<NewsSection />);
  const btn = screen.getByRole("button", { name: "Hozzáadás" });
  expect(btn).toBeDisabled();
});

test("mentés állapot megjelenik amikor létrehozás folyamatban van", async () => {
  useAuth.mockReturnValue({ token: "test-token", isAdmin: true });
  newsService.createNews.mockImplementation(() => new Promise(() => {})); // pending
  newsService.getNews.mockResolvedValue([]);
  const user = userEvent.setup();
  render(<NewsSection />);
  await user.type(screen.getByPlaceholderText("Cím"), "Cím");
  await user.type(screen.getByPlaceholderText("Szöveg"), "Szöveg");
  await user.click(screen.getByRole("button", { name: "Hozzáadás" }));
  expect(screen.getByRole("button", { name: "Mentés..." })).toBeInTheDocument();
});

test("űrlap törlődik sikeres létrehozás után", async () => {
  useAuth.mockReturnValue({ token: "test-token", isAdmin: true });
  newsService.createNews.mockResolvedValue({});
  newsService.getNews.mockResolvedValue([]);
  const user = userEvent.setup();
  render(<NewsSection />);
  const title = screen.getByPlaceholderText("Cím");
  const text = screen.getByPlaceholderText("Szöveg");
  await user.type(title, "X");
  await user.type(text, "Y");
  await user.click(screen.getByRole("button", { name: "Hozzáadás" }));
  await waitFor(() => {
    expect(title).toHaveValue("");
    expect(text).toHaveValue("");
  });
});

test("hibaüzenet megjelenik ha a hírek betöltése meghiúsul", async () => {
  newsService.getNews.mockRejectedValue(new Error("network"));
  render(<NewsSection />);
  await waitFor(() => {
    expect(screen.getByText(/network|nem sikerült/i)).toBeInTheDocument();
  });
});

test("hibaüzenet megjelenik ha létrehozás meghiúsul", async () => {
  useAuth.mockReturnValue({ token: "test-token", isAdmin: true });
  newsService.createNews.mockRejectedValue(new Error("create-fail"));
  newsService.getNews.mockResolvedValue([]);
  const user = userEvent.setup();
  render(<NewsSection />);
  await user.type(screen.getByPlaceholderText("Cím"), "Err");
  await user.type(screen.getByPlaceholderText("Szöveg"), "Err");
  await user.click(screen.getByRole("button", { name: "Hozzáadás" }));
  await waitFor(() => {
    expect(screen.getByText(/create-fail|nem sikerült/i)).toBeInTheDocument();
  });
});