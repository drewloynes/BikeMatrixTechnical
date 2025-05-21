import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom'

function ListBikes() {
    const [bikes, setBikes] = useState();

    const fillBikes = async () => {
        const response = await fetch('api/bikes');
        if (response.ok) {
            console.log(response)
            const data = await response.json();
            setBikes(data);
        }
    }

    const deleteBike = async (email) => {
        const response = await fetch(`/api/bikes/${encodeURIComponent(email)}`, {
        method: 'DELETE',
        headers: { 'Content-Type': 'application/json' }});

        if (response.ok) {
        fillBikes();
        } else {
        const text = await response.text();
        console.log(text)
        }
    };

    useEffect(() => {
        fillBikes();
    }, []);

    const navigate = useNavigate()

    const contents = bikes === undefined
        ? <p>Loading... </p>
        : <table className="table table-striped mx-8">
            <thead>
                <tr>
                    <th className="border-b-2 border-gray-500">E-Mail</th>
                    <th className="border-b-2 border-gray-500">Brand</th>
                    <th className="border-b-2 border-gray-500">Model</th>
                    <th className="border-b-2 border-gray-500">Year</th>
                </tr>
            </thead>
            <tbody>
                {bikes.map(bike =>
                    <tr key={bike.id}>
                        <td className="border-2 border-gray-200">{bike.email}</td>
                        <td className="border-2 border-gray-200">{bike.brand}</td>
                        <td className="border-2 border-gray-200">{bike.model}</td>
                        <td className="border-2 border-gray-200">{bike.year}</td>
                        <td className="w-45"><button className="w-40 bg-black" onClick={() => {navigate(`/update-bike/${bike.id}`)}}>Update</button></td>
                        <td className="w-45"><button className="w-40" onClick={() => {deleteBike(bike.id)}}>Delete</button></td>
                    </tr>
                )}
            </tbody>
        </table>;

    return (
        <div className="w-screen flex flex-col text-center justify-center">
            <h1 id="tableLabel" className="mb-3 font-extrabold">All Bikes In Database</h1>
            {contents}
        </div>
    );
}

export default ListBikes;