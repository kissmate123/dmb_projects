import React from "react";
import CharacterCard from "../components/CharacterCard";

const Characters = () => {
  const characters = [
    { name: "DOOM SLAYER", img: "/assets/images/doomslayer_gif.jpg" },
    { name: "BATMAN", img: "/assets/images/batman_gif.gif", type: "batman" },
    { name: "SCORPION", img: "/assets/images/mortal_k_gif.gif", type: "scorpion" },
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
