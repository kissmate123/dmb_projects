import React from "react";
import { Link } from "react-router-dom";

const Home = () => {
  return (
    <div>
      <div id="kosz1">
        <h2 id="koszsz">Üdvözlet a The Pixel Realms világában</h2>
      </div>

      <div className="tartalom1">
        <p>
          A karakter kinézetét pályától függetlenül mindig változtathatod!{" "}
          <Link to="/karakterek">Skinek megtekintése →</Link>
        </p>
        <p>A pályákra bármilyen karakter kinézettel beléphetsz.</p>
      </div>
    </div>
  );
};

export default Home;
