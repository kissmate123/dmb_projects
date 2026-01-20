import React from "react";
import { Link, NavLink } from "react-router-dom";

const Navbar = () => {
  return (
    <nav className="navbar">
      <div className="nav-container">

        <Link className="nav-logo" to="/">
          <img 
            src="/assets/images/logo_favi.png"   
            alt="Logo"
            className="logo-image"
            style={{ width: "40px", marginRight: "8px", borderRadius: "5px" }}
          />
          The Pixel Realms
        </Link>

        <ul className="nav-links">
          <li><NavLink to="/">Otthon</NavLink></li>
          <li><NavLink to="/karakterek">Karakterek</NavLink></li>
          <li><NavLink to="/login">Bejelentkezés</NavLink></li>
        </ul>

      </div>
    </nav>
  );
};

export default Navbar;
