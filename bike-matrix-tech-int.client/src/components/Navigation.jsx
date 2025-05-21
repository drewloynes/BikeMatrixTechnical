import { useNavigate } from 'react-router-dom';
import { Outlet } from "react-router-dom";

const Navigation = () => {
  const navigate = useNavigate();

  return (
    <>
      <nav className="flex-row my-5 mx-10">
        <button className="mx-4" onClick={() => {navigate("/")}}>List Bikes</button>
        <button className="mx-4" onClick={() => {navigate("/create-bike")}}>Create Bike</button>
      </nav>

      <Outlet />
    </>
  )
};

export default Navigation;