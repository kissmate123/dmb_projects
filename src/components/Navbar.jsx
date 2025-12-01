import React from "react";
import { Link, NavLink } from "react-router-dom";

const Navbar = () => {
  return (
    <nav className="navbar">
      <div className="nav-container">

        <Link className="nav-logo" to="/">
          <img 
            src="/assets/images/logo.jpg"   // <-- NINCS import, public-ból jön
            alt="Logo"
            className="logo-image"
            style={{ width: "35px", marginRight: "8px", borderRadius: "5px" }}
          />
          Logo
        </Link>

        <ul className="nav-links">
          <li><NavLink to="/">Home</NavLink></li>
          <li><NavLink to="/karakterek">Karakterek</NavLink></li>
          <li><NavLink to="/login">Login</NavLink></li>
        </ul>

      </div>
    </nav>
  );
};

export default Navbar;
