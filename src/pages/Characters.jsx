import React from "react";
import CharacterCard from "../components/CharacterCard";

const Characters = () => {
  const characters = [
    { name: "DOOM SLAYER", img: "public/assets/gifs/doomslyer_gif.jpg"},
    { name: "BATMAN", img: "/public/assets/gifs/batman_gif.gif", type: "batman" },
    { name: "SCORPION", img: "/public/assets/gifs/mortal_k_gif.gif", type: "scorpion" },
  ];

  return (
    <div>
      <div className="elvalaszto">
        <h3>Alapkarakterek</h3>
      </div>

      <div className="container">
        {characters.map((c, idx) => (
          <CharacterCard key={idx} {...c} />
        ))}
      </div>
    </div>
  );
};

export default Characters;
