import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useEffect, useState } from 'react';
import * as z from 'zod';

const bikeSchema = z.object({
  email: z.string().email(),
  brand: z.enum(['Trek', 'Giant', 'Canyon'], { message: 'Invalid brand' }),
  model: z.string().refine(
    (val) =>
      ['Dude', 'Exceed'].includes(val) ||
      ['Defy', 'Escape'].includes(val) ||
      ['Boone', 'District'].includes(val),
    { message: 'Invalid model for selected brand' }
  ),
  year: z.enum(['2020', '2024'], { message: 'Invalid year' }),
  id: z.number().optional(),
});

const brandModelMap = {
  Canyon: ['Dude', 'Exceed'],
  Giant: ['Defy', 'Escape'],
  Trek: ['Boone', 'District'],
};
const years = ['2020', '2024'];

// Form for creating or updating bike information
export default function BikeForm({ id = null, onSuccess }) {
  const [status, setStatus] = useState(null);

  const {
    register,
    handleSubmit,
    watch,
    reset,
    formState: { errors },
  } = useForm({
    resolver: zodResolver(bikeSchema),
    defaultValues: {
      email: '',
      brand: '',
      model: '',
      year: null,
    },
  });

  const brand = watch('brand');

  // Fetch and fill the bike details if the Id was included
  useEffect(() => {
    if (id) {
      fetch(`/api/bikes/${id}`)
        .then((res) => res.json())
        .then((bike) => reset(bike))
        .catch(() => setStatus('Failed to load bike.'));
    }
  }, [id, reset]);

  const onSubmit = async (data) => {
    const method = id ? 'PUT' : 'POST';
    const url = id ? `/api/bikes/${id}` : '/api/bikes';
    const payload = id ? { ...data, id: parseInt(id, 10) } : data;

    try {
      const response = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      });

      if (response.ok) {
        setStatus(id ? 'Bike updated!' : 'Bike created!');
        // Reset create form
        if (!id) {
          reset();
        }
        if (onSuccess) {
          onSuccess();
        }
      } else {
        // Response not ok
        const errText = await response.text();
        setStatus(`Error: ${errText}`);
      }
    } catch (err) {
      // Fetch likely failed
      setStatus(`Network error: ${err.message}`);
    }
  };

  return (
    <div className="flex justify-center px-4">
        <div className="w-full max-w-xl p-6 bg-white shadow-xl rounded-2xl mt-10">
            <h2 className="text-2xl font-semibold mb-6 text-gray-800">{id ? 'Update Bike' : 'Create Bike'}</h2>

            <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">

                <div>
                  <label className="block mb-1 font-medium text-gray-700" htmlFor="email">Email:</label>
                  <input
                      id="email"
                      type="email"
                      {...register('email')}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring focus:border-blue-400"
                  />
                  {errors.email && <p className="text-sm text-red-500 mt-1">{errors.email.message}</p>}
                </div>

                <div>
                  <label className="block mb-1 font-medium text-gray-700"  htmlFor="brand">Brand:</label>
                  <select
                      id="brand"
                      {...register('brand')}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring focus:border-blue-400"
                  >
                      <option value="">-- Select Brand --</option>
                      {Object.keys(brandModelMap).map((b) => (
                      <option key={b} value={b}>
                          {b}
                      </option>
                      ))}
                  </select>
                  {errors.brand && <p className="text-sm text-red-500 mt-1">{errors.brand.message}</p>}
                </div>

                <div>
                  <label className="block mb-1 font-medium text-gray-700"  htmlFor="model">Model:</label>
                  <select
                      id="model"
                      {...register('model')}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring focus:border-blue-400"
                  >
                      <option value="">-- Select Model --</option>
                      {brand && brandModelMap[brand]?.map((model) => (
                      <option key={model} value={model}>
                          {model}
                      </option>
                      ))}
                  </select>
                  {errors.model && <p className="text-sm text-red-500 mt-1">{errors.model.message}</p>}
                </div>

                <div>
                  <label className="block mb-1 font-medium text-gray-700"  htmlFor="year">Year:</label>
                  <select
                      id="year"
                      {...register('year')}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring focus:border-blue-400"
                  >
                      <option value="">-- Select Year --</option>
                      {years.map((year) => (
                      <option key={year} value={year}>
                          {year}
                      </option>
                      ))}
                  </select>
                  {errors.year && <p className="text-sm text-red-500 mt-1">{errors.year.message}</p>}
                </div>

                <button
                type="submit"
                className="w-full text-black font-semibold py-2 px-4 rounded-lg shadow-md transition duration-200"
                >
                  {id ? 'Update Bike' : 'Create Bike'}
                </button>

                {status && <p className="text-sm text-center mt-4 text-gray-600">{status}</p>}
            </form>
        </div>
    </div>
  );
}
