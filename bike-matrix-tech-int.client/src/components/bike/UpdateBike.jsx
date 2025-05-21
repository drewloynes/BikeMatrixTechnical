import { useParams, useNavigate } from 'react-router-dom';
import BikeForm from './BikeForm';

const UpdateBike = () => {
  const { id } = useParams();
  const navigate = useNavigate();

  return <BikeForm id={id} onSuccess={() => navigate('/')} />;
};

export default UpdateBike;
