import React from "react";

const Footer = () => {
  return (
    <footer>
      <div className="footerContainer">
        <div>
          <p>dmb projects©</p>

          <table>
            <tbody>
              <tr>
                <th className="footer-table">
                  <p className="footer-legal-left">
                    A textúrák és a megjelenés (kinézet) tulajdonjogát fenntartjuk.
                  </p>
                  <p className="footer-legal-left">
                    Ezek a grafikai elemek a készítő/üzemeltető kizárólagos szellemi tulajdonát képezik,
                    így azok másolása, terjesztése vagy felhasználása csak előzetes írásbeli
                    engedéllyel lehetséges.©
                  </p>
                </th>

                <th>
                  <a
                    href="https://code.visualstudio.com"
                    target="_blank"
                    rel="noreferrer"
                  >
                    <img
                      className="footer-img"
                      src="/assets/images/visualcode_logo_footer.png"
                      alt="visual studio code"
                    />
                  </a>

                  <img
                    className="footer-img"
                    src="/assets/images/dmb_logo_footer.png"
                    alt="dmb"
                  />

                  <a
                    href="https://visualstudio.microsoft.com"
                    target="_blank"
                    rel="noreferrer"
                  >
                    <img
                      className="footer-img"
                      src="/assets/images/visualstudio_logo_footer.png"
                      alt="visual studio"
                    />
                  </a>
                </th>

                <th className="footer-table">
                  <p className="footer-legal-right">
                    A dmb projects© csapat három fejlesztőből áll, <br/>
                    Sütő Dominik, Kiss Máté, Tamás Bence
                  </p>
                  <p className="footer-legal-right"></p>
                </th>
              </tr>
            </tbody>
          </table>

          <a id="fel" href="#top">vissza a tetejére</a>
        </div>
      </div>
    </footer>
  );
};

export default Footer;