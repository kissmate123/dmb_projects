import React from "react";
import CharacterCard from "../components/CharacterCard";

const Characters = () => {
  const characters = [
    {
      id: "borien",
      name: "BORIEN",
      img: "/assets/gifs/Karakter_Alap.gif",
      type: "Borien",
      category: "hero",
      description: "A Pixel Realms bátor főhőse.",
    },
    {
      id: "batman",
      name: "BATMAN",
      img: "/assets/gifs/batman_gif.gif",
      type: "batman",
      category: "enemy",
      description: "Sötét lovag stílusú ellenfél, gyors és alattomos.",
    },
    {
      id: "scorpion",
      name: "SCORPION",
      img: "/assets/gifs/mortal_k_gif.gif",
      type: "scorpion",
      category: "enemy",
      description: "Tűz alapú támadásokkal operál, közelharcban veszélyes.",
    },
  ];

  const hero = characters.find((c) => c.category === "hero");
  const enemies = characters.filter((c) => c.category === "enemy");

  return (
    <div>
      {/* 1. sor: FŐHŐS */}
      <div className="container container-single">
        {hero && <CharacterCard {...hero} />}
      </div>

      {/* Elválasztó */}
      <div className="elvalaszto">
      </div>

      {/* 2. sor: ELLENSÉGEK */}
      <div className="container container-grid">
        {enemies.map((c) => (
          <CharacterCard key={c.id} {...c} />
        ))}
      </div>
    </div>
  );
};

export default Characters;
