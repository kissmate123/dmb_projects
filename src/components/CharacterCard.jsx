import React from "react";

const CharacterCard = ({ name, img, type, description }) => {
  return (
    <div className={`karakter ${type || ""}`}>
      <img src={img} alt={name} />
      <h3>{name}</h3>

      {/* Hover overlay */}
      {description && (
        <div className="karakter-desc">
          <p>{description}</p>
        </div>
      )}
    </div>
  );
};

export default CharacterCard;
