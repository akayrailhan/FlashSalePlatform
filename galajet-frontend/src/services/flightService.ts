import axios from 'axios';

import type { IFlight } from '../types/flight';

const FLIGHTS_URL =
    'https://galajet-api-b2eyb4b6dbexfnd2.italynorth-01.azurewebsites.net/api/flights';

export const getFlights = async (): Promise<IFlight[]> => {
    const response = await axios.get<IFlight[]>(FLIGHTS_URL);
    return response.data;
};
