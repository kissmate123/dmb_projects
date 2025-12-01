import React from "react";

const CharacterCard = ({ name, img, type }) => {
  return (
    <div className={`karakter ${type || ""}`}>
      <img src={img} alt={name} />
      <h3>{name}</h3>
    </div>
  );
};

export default CharacterCard;
