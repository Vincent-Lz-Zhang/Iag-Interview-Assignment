// Fetch Vehicle Summary
const fetchVehicleSummary = async (make) => {
  const response = await fetch(
    `http://localhost:5001/api/v1/vehicle-checks/makes/${make}`
  );
  if (response.ok) {
    const summary = await response.json();
    return summary;
  } else if (response.status === 404) {
    console.log("404 error occurs when fetching Vehicle Summary.");
    const errorResponse = await response.json();
    throw new Error(
      `${
        errorResponse?.value?.detail ?? "The make could not be found."
      } \nTrace Id: ${errorResponse?.value?.traceId}`
    );
  } else {
    console.log(
      `${response.status} error occurs when fetching Vehicle Summary.`
    );
    throw new Error("Network error occurred.");
  }
};

export default fetchVehicleSummary;
