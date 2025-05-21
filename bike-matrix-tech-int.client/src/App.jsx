import { BrowserRouter, Routes, Route } from "react-router-dom";
import Navigation from "./components/Navigation";
import ListBikes from "./components/bike/ListBikes";
import CreateBike from "./components/bike/CreateBike";
import UpdateBike from "./components/bike/UpdateBike";
import NoPage from "./components/NoPage";

function App() {

    return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigation />}>
          <Route index element={<ListBikes />} />
          <Route path="create-bike" element={<CreateBike />} />
          <Route path="update-bike/:id" element={<UpdateBike />} />
          <Route path="*" element={<NoPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
    );
}

export default App;