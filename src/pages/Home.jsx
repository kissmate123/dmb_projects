import React from "react";
import { Link } from "react-router-dom";
import NewsSection from "../components/NewsSection";

const Home = () => {
  return (
    <div>
      <div className="trailer-section">
        <div className="trailer-text">
          <h3>The Pixel Realms előzetes</h3>
          <p>
            Nézd meg a legújabb előzetest, és pillants be a The Pixel Realms világába.
          </p>
        </div>

        <div className="trailer-video">
          <video id="trailer" src="/assets/images/Trailer.mp4" controls />
        </div>
      </div>


      <div className="tartalom1">
        <p>
          A The Pixel Realms játék egy oldal nézetes(2D) játék amely egy történet alapján működik.
          <a href="#tortenet" onClick={(e) => { e.preventDefault(); document.getElementById("tortenet").scrollIntoView({ behavior: "smooth" }); }}>Történet megtekintése ↓</a>
          <p>A játék irányítása egyszerű. Mindent amit tudnod kell a játékban megtalálod.</p>
          <img id="moves_img" src="/public/assets/images/moves.png" alt="moves" />

        </p>
        <p>Különböző karakterek és ellenségek vannak. <Link to="/karakterek"> Karakterek, ellenségek megtekintése →</Link></p>

      </div>

      <div className="elvalaszto">
      </div>
      <div className="tartalom1">
        {/* TÖRTÉNET */}
        <div id="tortenet" className="story-section">
          <div>
            <h2 className="story-title">THE PIXEL REALMS TÖRTÉNETE</h2>
          </div>

          <div className="story-cards">
            {/* 1. FEJEZET (jobbra nyílik) */}
            <article className="story-card story-card--left" style={{ "--accent": "#ff0000" }}>
              <div className="story-meta">1. fejezet</div>
              <h3 className="story-card-title">A támadás</h3>
              <p className="story-lead">
                Nyugalmas délelőtt a város piacterén. A békét szárnyas szörnyetegek törik meg...
              </p>

              <div className="story-more">
                <p>
                  Nyugalmas délelőtt a város piacterén.
                  <span className="tutorial">Alap mozgás tutorial</span>
                </p>

                <p>
                  Borien beszél <strong>Mirával</strong>, a pékkel
                  <span className="tutorial">HP és gyógyítás bemutatása</span>,
                  majd <strong>Ariccal</strong>, a város kovácsával
                  <span className="tutorial">fegyverzet és páncél felszerelése</span>.
                </p>

                <p>
                  Tekintete a palota tornyára téved, ahol <strong>Elira hercegnő</strong> él,
                  amikor szárnyas szörnyetegek – <strong>Grivakok</strong> – jelennek meg a láthatáron.
                </p>

                <p>
                  Néhány a torony felé tart, miközben a falakon át <strong>Brogurok</strong> törnek be a városba.
                  <strong>Eldon</strong> és a városi őrség feltartja őket, hogy Borien a hercegnőhöz juthasson.
                </p>

                <p>
                  Későn érkezik. A szoba feldúlva, az ablak betörve, a Grivakok már a városon túl repülnek Elirával.
                </p>

                <p>
                  Borien kiugrik az ablakon és áttör a városon.
                  <span className="tutorial">harci tutorial</span>
                </p>

                <p className="dramatic">
                  A nagy kapunál Borien haldokló mesterére, <strong>Eldonra</strong> talál.
                  Utolsó erejével az íját adja át, és a hercegnő megmentésére sürgeti.
                </p>

                <p className="end">A nagy kapu átlépésével a fejezet véget ér.</p>
              </div>
            </article>

            {/* 2. FEJEZET (két oldalra nyílik) */}
            <article className="story-card story-card--middle" style={{ "--accent": "#a855f7" }}>
              <div className="story-meta">2. fejezet</div>
              <h3 className="story-card-title">A Voryndal erdő</h3>
              <p className="story-lead">
                Borien a sűrű erdőn át üldözi a Grivakokat, de a nyomok eltűnnek...
              </p>

              <div className="story-more">
                <p>
                  Borien elhagyja a város kapuit, és tovább üldözi a Grivakokat.
                  Útja a sűrű <strong>Voryndal erdőn</strong> át vezet.
                </p>

                <p>
                  Egy tisztás közepére ér, ahol <strong>Brogurok</strong> és <strong>Fenwargok</strong> állomásoznak.
                  Borien leszámol velük <span className="tutorial">íj használat tutorial</span>.
                </p>

                <p>
                  A Grivakok nyomai egyre halványabbak. Az erdő fái olyan sűrűek, hogy Borien végül teljesen elveszíti őket.
                </p>

                <p>
                  Hirtelen újabb <strong>Fenwargok</strong>, valamint két <strong>Thralog</strong> bukkan fel előtte.
                  A túlerő túl nagy, ezért Borien menekülni kényszerül.
                </p>

                <p>
                  Egy apró házat pillant meg, ahová berohan. A házat mágikus erőburok védi.
                </p>

                <p>
                  A ház <strong>Selvaráé</strong>, a boszorkányé. Selvara elmondja, hová viszik a hercegnőt – <strong>egy szeme árán</strong>.
                </p>

                <p className="dramatic">
                  Borien elfogadja az alkut. Fél szemét elveszítve, de eltökélten folytatja útját.
                </p>

                <p className="end">
                  Borien nem messze, egy sziklaperem alatt ver tábort, felkészülve az előtte álló veszélyekre.
                </p>
              </div>
            </article>

            {/* 3. FEJEZET (balra nyílik) */}
            <article className="story-card story-card--right" style={{ "--accent": "#ff0000" }}>
              <div className="story-meta">3. fejezet</div>
              <h3 className="story-card-title">Az erdő szíve</h3>
              <p className="story-lead">
                A legsötétebb részen Borien olyan ellenféllel találkozik, aki próbára teszi...
              </p>

              <div className="story-more">
                <p>
                  Reggel virrad Borienre, de a nap fénye nem hatol át a lombokon.
                  Ez már a <strong>Voryndal erdő</strong> legsötétebb és leggonoszabb része.
                </p>

                <p>
                  Borien tovább indul, amikor egy magányos, feldühödött <strong>Thralog</strong> állja útját –
                  <span className="tutorial">nagy ellenfél harci tutorial</span>.
                </p>

                <p>
                  A küzdelem kemény, de Borien győz. A harc megtanítja, milyen egy valóban erős ellenféllel szembenézni.
                </p>

                <p>
                  Nem sokkal később <strong>Gryndalorra</strong>, az Entre bukkan, akit a sötétség szolgái támadnak. Borien megmenti őt.
                </p>

                <p>
                  Hálából Gryndalor elkíséri a barlangig, és útközben mindent elmond <strong>Vathryxról</strong> és terveiről.
                </p>

                <p className="dramatic">
                  Elárulja, hogy Elira hercegnő ereiben anyai ágon mágikus vér folyik, amelyre Vathryxnak szüksége van.
                </p>

                <p className="end">A fejezet itt véget ér.</p>
              </div>
            </article>
          </div>
        </div>
      </div>
      <div className="tartalom1">
        <NewsSection />
      </div>

    </div>
  );
};

export default Home;
