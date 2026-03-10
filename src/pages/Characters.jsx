import React from "react";
import CharacterCard from "../components/CharacterCard";

const Characters = () => {


  return (
  <div>
    <div className="tartalom1">
      <h2 className="title">BORIEN</h2>

      <div className="epizode-row">
        <div className="epizode-text">
          <img className="karakter-img" src="/assets/gifs/Karakter_Alap.gif" alt="alap_karakter" />

        </div>

        <div className="epizode-media">
          <p>
            Borien elhagyja a város kapuit, és tovább üldözi a Grivakokat. Útja a sűrű{" "}
            <strong>Voryndal erdőn</strong> át vezet.
          </p>

          <p>
            Egy tisztás közepére ér, ahol <strong>Brogurok</strong> és <strong>Fenwargok</strong>{" "}
            állomásoznak. Borien leszámol velük. (íj használat tutorial)
          </p>

          <p>
            A Grivakok nyomai egyre halványabbak. Az erdő fái olyan sűrűek, hogy Borien végül teljesen
            elveszíti őket.
          </p>

          <p>
            Hirtelen újabb <strong>Fenwargok</strong>, valamint két <strong>Thralog</strong> bukkan fel
            előtte. A túlerő túl nagy, ezért Borien menekülni kényszerül.
          </p>

          <p>Egy apró házat pillant meg, ahová berohan. A házat mágikus erőburok védi.</p>

          <p>
            A ház <strong>Selvaráé</strong>, a boszorkányé. Selvara elmondja, hová viszik a hercegnőt –{" "}
            <strong>egy szeme árán</strong>.
          </p>

          <p className="dramatic">
            Borien elfogadja az alkut. Fél szemét elveszítve, de eltökélten folytatja útját.
          </p>

          <p className="end">
            Borien nem messze, egy sziklaperem alatt ver tábort, felkészülve az előtte álló
            veszélyekre.
          </p>
        </div>
      </div>
    </div>

    <div className="tartalom2" id="tortenet">
        <h2 className="title">Ellenséges lények</h2>
      </div>

    <div className="tartalom1">
      <h2 className="title">BROGUR</h2>

      <div className="epizode-row">
        <div className="epizode-text">
          <img className="karakter-img" src="/assets/gifs/Karakter_Brogur.gif" alt="brogur_karakter" />

        </div>

        <div className="epizode-media">
          <p>
            Borien elhagyja a város kapuit, és tovább üldözi a Grivakokat. Útja a sűrű{" "}
            <strong>Voryndal erdőn</strong> át vezet.
          </p>

          <p>
            Egy tisztás közepére ér, ahol <strong>Brogurok</strong> és <strong>Fenwargok</strong>{" "}
            állomásoznak. Borien leszámol velük. (íj használat tutorial)
          </p>

          <p>
            A Grivakok nyomai egyre halványabbak. Az erdő fái olyan sűrűek, hogy Borien végül teljesen
            elveszíti őket.
          </p>

          <p>
            Hirtelen újabb <strong>Fenwargok</strong>, valamint két <strong>Thralog</strong> bukkan fel
            előtte. A túlerő túl nagy, ezért Borien menekülni kényszerül.
          </p>

          <p>Egy apró házat pillant meg, ahová berohan. A házat mágikus erőburok védi.</p>

          <p>
            A ház <strong>Selvaráé</strong>, a boszorkányé. Selvara elmondja, hová viszik a hercegnőt –{" "}
            <strong>egy szeme árán</strong>.
          </p>

          <p className="dramatic">
            Borien elfogadja az alkut. Fél szemét elveszítve, de eltökélten folytatja útját.
          </p>

          <p className="end">
            Borien nem messze, egy sziklaperem alatt ver tábort, felkészülve az előtte álló
            veszélyekre.
          </p>
        </div>
      </div>
    </div>
  </div>
  );
};

export default Characters;
