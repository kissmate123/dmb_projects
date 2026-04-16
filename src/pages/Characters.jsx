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

        <div className="character-description">
          <p>
            Borien a történet bátor főhőse, egy fiatal harcos és íjász, aki mindennél fontosabbnak tartja szerelmét, Elira hercegnőt. 
          </p>
          <p> 
            Kitartó, önfeláldozó és elszánt, még a legsötétebb veszélyekkel is szembeszáll, hogy megmentse őt és megállítsa Vathryx tervét.
          </p>
        </div>
      </div>
   

  
      <h2 className="title">DORIN</h2>

      <div className="epizode-row">
        <div className="epizode-text">
          <img className="karakter-img" src="/assets/gifs/karakter_dorin.gif" alt="dorin_karakter" />

        </div>

        <div className="character-description">
          <p>
            Dorin megbízható és tapasztalt harcos, aki erejével és hűségével mindig társai mellett áll a legnehezebb időkben is.
          </p>
          <p> 
            Komor, kemény természetű férfi, de a szíve mélyén nemes lélek, aki kész bármikor életét kockáztatni azokért, akiket fontosnak tart.
          </p>
        </div>
      </div>
  

      <h2 className="title">ELDON</h2>

      <div className="epizode-row">
        <div className="epizode-text">
          <img className="karakter-img" src="/assets/gifs/karakter_eldon.gif" alt="eldon_karakter" />

        </div>

        <div className="character-description">
          <p>
            Eldon Borien mestere, a város egyik legbölcsebb és legvitézebb harcosa, aki hosszú éveken át készítette fel tanítványát a rá váró megpróbáltatásokra.
          </p>
          <p> 
            Bölcs, fegyelmezett és önfeláldozó ember, aki utolsó erejével is Borient segíti, hogy megmenthesse Elira hercegnőt és beteljesítse sorsát.
          </p>
        </div>
      </div>
  


  
      <h2 className="title">MIRA</h2>

      <div className="epizode-row">
        <div className="epizode-text">
          <img className="karakter-img" src="/assets/gifs/karakter_mira.gif" alt="mira_karakter" />

        </div>

        <div className="character-description">
          <p>
            Mira a város kedves és gondoskodó pékje, aki nemcsak friss kenyérrel, hanem gyógyító tudásával is segíti az embereket a nehéz időkben.
          </p>
          <p> 
            Jószívű, türelmes és bátor asszony, aki a káosz közepén is igyekszik reményt és biztonságot nyújtani mindazoknak, akik hozzá fordulnak.
          </p>
        </div>
      </div>
    </div>

    <div className="elvalaszto"></div>

    <div className="tartalom1">
        <h2 className="title">Ellenséges lények</h2>
      </div>

      <div className="elvalaszto"></div>

    <div className="tartalom1">
      <h2 className="title">BROGUR</h2>

      <div className="epizode-row">
        <div className="epizode-text">
          <img className="karakter-img" src="/assets/gifs/Karakter_Brogur.gif" alt="brogur_karakter" />

        </div>

        <div className="character-description">
          <p>
            A Brogurok brutális, falmászó szörnyetegek, akik nyers erejükkel és kegyetlenségükkel támadják meg a várost.
          </p>
          <p>
            Vad, veszélyes ellenfelek, akik a sötétség szolgáiként pusztítanak, és az útjukba kerülőket könyörtelenül lemészárolják.
          </p>
        </div>
      </div>
    

   
      <h2 className="title">SKULK</h2>

      <div className="epizode-row">
        <div className="epizode-text">
          <img className="karakter-img" src="/assets/gifs/karakter_skeleton.gif" alt="skulk_karakter"/>

        </div>

        <div className="character-description">
          <p>
            A Skulk lopakodó, sunyi teremtmény, amely a sötétséget és a rejtőzködést használja előnyére.
          </p>
          <p>
            Nem a nyers ereje teszi veszélyessé, hanem a gyorsasága, alattomossága és az, hogy váratlanul csap le áldozataira.
          </p>
        </div>
      </div>
    

    
      <h2 className="title">GRIVAK</h2>

      <div className="epizode-row">
        <div className="epizode-text">
          <img className="karakter-img" src="/assets/gifs/karakter_grivak.gif" alt="grivak_karakter"/>

        </div>

        <div className="character-description">
          <p>
            A Grivalk szárnyas, félelmetes ragadozó, a sötétség egyik legveszélyesebb szolgája.
          </p>
          <p>
            Gyors, agresszív és könyörtelen ellenfél, amely a levegőből támadva hurcolja el prédáját, és rettegést kelt mindenhol, ahol megjelenik.
          </p>
        </div>
      </div>
    </div>
  </div>
  );
};

export default Characters;
