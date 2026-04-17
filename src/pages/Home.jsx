import React from "react";
import { Link } from "react-router-dom";
import NewsSection from "../components/NewsSection";

const Home = () => {
  return (
    <div>
      <div className="trailer-section">
        <div className="trailer-text">
          <h2 className="title">The Pixel Realms előzetes</h2>
          <p>Nézd meg a legújabb előzetest, és pillants be a The Pixel Realms világába.</p>
        </div>

        <div className="trailer-video">
          <video id="trailer" src="/assets/images/Trailer.mp4" controls />
        </div>
      </div>

      <div className="tartalom1">
        <p>
          A The Pixel Realms játék egy oldal nézetes(2D) játék amely egy történet alapján működik.{" "}
          <a
            href="#tortenet"
            onClick={(e) => {
              e.preventDefault();
              document.getElementById("tortenet")?.scrollIntoView({ behavior: "smooth" });
            }}
          >
            Történet megtekintése ↓
          </a>
        </p>

        <p>A játék irányítása egyszerű. Mindent amit tudnod kell a játékban megtalálod.</p>

        <img id="moves_img" src="/assets/images/moves.png" alt="moves" />

        <p>
          Különböző karakterek és ellenségek vannak.{" "}
          <Link to="/karakterek">Karakterek, ellenségek megtekintése →</Link>
        </p>
      </div>

      <div className="gap"></div>

      <div className="tartalom1">
        <h2 className="title">The Pixel Realms története</h2>
      </div>

      <div className="tartalom1">
        <h3 style={{ fontSize: "1.5rem", fontStyle: "italic" }} className="title">1. fejezet – A támadás</h3>

        <div className="epizode-row">
          <div className="epizode-text">
            <p>Nyugalmas délelőtt a város piacterén. (Alap mozgás tutorial)</p>

            <p>
              Borien beszél <strong>Mirával</strong>, a pékkel (HP és gyógyítás bemutatása), majd{" "}
              <strong>Ariccal</strong>, a város kovácsával fegyverzet és páncél felszerelése.
            </p>

            <p>
              Tekintete a palota tornyára téved, ahol <strong>Elira hercegnő</strong> él, amikor szárnyas
              szörnyetegek – <strong>Grivakok</strong> – jelennek meg a láthatáron.
            </p>

            <p>
              Néhány a torony felé tart, miközben a falakon át <strong>Brogurok</strong> törnek be a
              városba. <strong>Eldon</strong> és a városi őrség feltartja őket, hogy Borien a hercegnőhöz
              juthasson.
            </p>

            <p>
              Későn érkezik. A szoba feldúlva, az ablak betörve, a Grivakok már a városon túl repülnek
              Elirával.
            </p>

            <p>Borien kiugrik az ablakon és áttör a városon. (harci tutorial)</p>

            <p>
              A nagy kapunál Borien haldokló mesterére, <strong>Eldonra</strong> talál. Utolsó erejével az
              íját adja át, és a hercegnő megmentésére sürgeti.
            </p>

            <p>A nagy kapu átlépésével a fejezet véget ér.</p>
          </div>

          <div className="epizode-media">
            <img className="epizode-img" src="/assets/images/page_back.png" alt="1. fejezet" />
          </div>
        </div>
      

      
        <h2 style={{ fontSize: "1.5rem", fontStyle: "italic" }} className="title">2. fejezet – A Voryndal erdő</h2>

        <div className="epizode-row">
          <div className="epizode-text">
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

            <p>
              Borien elfogadja az alkut. Fél szemét elveszítve, de eltökélten folytatja útját.
            </p>

            <p>
              Borien nem messze, egy sziklaperem alatt ver tábort, felkészülve az előtte álló
              veszélyekre.
            </p>
          </div>

          <div className="epizode-media">
            <img className="epizode-img" src="/assets/images/page_back.png" alt="2. fejezet" />
          </div>
        </div>
     
        <h2 style={{ fontSize: "1.5rem", fontStyle: "italic" }} className="title">3. fejezet – Az erdő szíve</h2>

        <div className="epizode-row">
          <div className="epizode-text">
            <p>
              Reggel virrad Borienre, de a nap fénye nem hatol át a lombokon. Ez már a{" "}
              <strong>Voryndal erdő</strong> legsötétebb és leggonoszabb része.
            </p>

            <p>
              Borien tovább indul, amikor egy magányos, feldühödött <strong>Thralog</strong> állja útját.
              (nagy ellenfél harci tutorial)
            </p>

            <p>
              A küzdelem kemény, de Borien győz. A harc megtanítja, milyen egy valóban erős ellenféllel
              szembenézni.
            </p>

            <p>
              Nem sokkal később <strong>Gryndalorra</strong>, az Entre bukkan, akit a sötétség szolgái
              támadnak. Borien megmenti őt.
            </p>

            <p>
              Hálából Gryndalor elkíséri a barlangig, és útközben mindent elmond <strong>Vathryxról</strong>{" "}
              és terveiről.
            </p>

            <p>
              Elárulja, hogy Elira hercegnő ereiben anyai ágon mágikus vér folyik, amelyre Vathryxnak
              szüksége van.
            </p>

            <p>A fejezet itt véget ér.</p>
          </div>

          <div className="epizode-media">
            <img className="epizode-img" src="/assets/images/page_back.png" alt="3. fejezet" />
          </div>
        </div>
      </div>

      <div className="gap"></div>

      <div className="tartalom1">
        <NewsSection />
      </div>
    </div>
  );
};

export default Home;