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
          A The Pixel Realms játék egy oldal nézetes(2D) játék amely egy történet alapján működik.
          <a href="#tortenet"onClick={(e) => {e.preventDefault(); document.getElementById("tortenet").scrollIntoView({ behavior: "smooth" });}}>Történet megtekintése ↓</a>
        
          
        </p>
        <p><Link to="/karakterek">Karakterek megtekintése →</Link></p>
      </div>

      <div className="elvalaszto">
        <p>dmb projects©</p>
      </div>
      <p id="tortenet"></p>
      <div id="kosz1">
        <h2 id="koszsz">The Pixel Realms története</h2>
      </div>
      <div className="tartalom-wrapper">

        {/* 1. FEJEZET – BAL OLDAL */}
        <div className="tartalom1">
          <h2>1. fejezet – A támadás</h2>

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
            amikor szárnyas szörnyetegek – <strong>Grivakok</strong> –
            jelennek meg a láthatáron.
          </p>

          <p>
            Néhány a torony felé tart, miközben a falakon át
            <strong>Brogurok</strong> törnek be a városba.
            <strong>Eldon</strong> és a városi őrség feltartja őket,
            hogy Borien a hercegnőhöz juthasson.
          </p>

          <p>
            Későn érkezik. A szoba feldúlva, az ablak betörve,
            a Grivakok már a városon túl repülnek Elirával.
          </p>

          <p>
            Borien kiugrik az ablakon és áttör a városon.
            A városi őrség nagy része elbukott –
            <span className="tutorial">harci tutorial</span>.
          </p>

          <p className="dramatic">
            A nagy kapunál Borien haldokló mesterére,
            <strong>Eldonra</strong> talál.
            Utolsó erejével az íját adja át,
            és a hercegnő megmentésére sürgeti.
          </p>

          <p className="end">
            A nagy kapu átlépésével a fejezet véget ér.
          </p>
        </div>

        {/* 2. FEJEZET – KÖZÉP */}
        <div className="tartalom1">
          <h2>2. fejezet – A Voryndal erdő</h2>

          <p>
            Borien elhagyja a város kapuit, és tovább üldözi a Grivakokat.
            Útja a sűrű <strong>Voryndal erdőn</strong> át vezet.
          </p>

          <p>
            Egy tisztás közepére ér, ahol
            <strong>Brogurok</strong> és <strong>Fenwargok</strong>
            állomásoznak.
            Borien leszámol velük
            <span className="tutorial">íj használat tutorial</span>.
          </p>

          <p>
            A Grivakok nyomai egyre halványabbak.
            Az erdő fái olyan sűrűek, hogy Borien végül teljesen elveszíti őket,
            és reménytelenül halad tovább előre.
          </p>

          <p>
            Hirtelen újabb <strong>Fenwargok</strong>, valamint két
            <strong>Thralog</strong> bukkan fel előtte.
            A túlerő túl nagy, ezért Borien menekülni kényszerül.
          </p>

          <p>
            Egy apró házat pillant meg, ahová berohan.
            A házat mágikus erőburok védi, amelyen a szörnyek nem tudnak áthatolni.
          </p>

          <p>
            A ház <strong>Selvaráé</strong>, a boszorkányé.
            Selvara elmondja, hová viszik a Grivakok a hercegnőt,
            és azt is, hogyan juthat el oda –
            <strong>egy szeme árán</strong>.
          </p>

          <p className="dramatic">
            Borien elfogadja az alkut.
            Fél szemét elveszítve, de eltökélten folytatja útját.
          </p>

          <p>
            Selvara elárulja, hogy a Voryndal erdő mélyén
            egy barlang rejtőzik, amelynek bejárata
            csak egy kulcsszó kimondásával nyílik meg.
            A boszorkány Borien fülébe súgja a szót,
            majd útjára engedi.
          </p>

          <p className="end">
            Borien nem messze, egy sziklaperem alatt ver tábort,
            felkészülve az előtte álló veszélyekre.
          </p>
        </div>
        {/* 3. FEJEZET – JOBB OLDAL */}
        <div className="tartalom1">
          <h2>3. fejezet – Az erdő szíve</h2>

          <p>
            Reggel virrad Borienre, de a nap fénye nem hatol át a lombokon.
            Ez már a <strong>Voryndal erdő</strong> legsötétebb és leggonoszabb része.
          </p>

          <p>
            Borien tovább indul az erdő közepe felé, amikor egy magányos,
            feldühödött <strong>Thralog</strong> állja útját –
            <span className="tutorial">nagy ellenfél harci tutorial</span>.
          </p>

          <p>
            A küzdelem kemény, de Borien győz.
            A harc megtanítja, milyen egy valóban erős ellenféllel szembenézni.
          </p>

          <p>
            Nem sokkal később egy hatalmas lényre bukkan:
            <strong>Gryndalorra</strong>, az Entre,
            akit a sötétség szolgái támadnak.
            Borien megmenti őt.
          </p>

          <p>
            Hálából Gryndalor elkíséri a barlangig,
            és útközben mindent elmond
            <strong>Vathryxról</strong> és terveiről.
          </p>

          <p className="dramatic">
            Elárulja, hogy Elira hercegnő ereiben
            anyai ágon mágikus vér folyik,
            amelyre Vathryxnak szüksége van,
            hogy megnyissa az átjárót
            és a sötétség seregeivel elpusztítsa a világ fényét.
          </p>

          <p>
            A barlang bejáratához érve
            Gryndalor visszatér az erdőbe,
            Borien pedig egyedül marad.
          </p>

          <p className="end">
            A fejezet itt véget ér.
          </p>
        </div>

      </div>
    </div>
  );
};

export default Home;
