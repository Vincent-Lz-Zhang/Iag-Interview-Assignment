import fetchVehicleSummary from "./fetchVehicleSummary";

beforeEach(() => {
  fetch.resetMocks();
});

describe("VehicleSummaryPlane Component", () => {
  it("should fetch vehicle summary when API works", async () => {
    const arbitraryMake = global.generateRandomString(10);
    const arbitraryModel = global.generateRandomString(10);
    const arbitraryYear = global.generateRandomInteger(1990, 2010);
    fetch.mockResponseOnce(
      JSON.stringify({
        make: arbitraryMake,
        models: [{ name: arbitraryModel, yearsAvailable: arbitraryYear }],
      })
    );

    const vehicleSummaryResponse = await fetchVehicleSummary();

    expect(vehicleSummaryResponse.make).toEqual(arbitraryMake);
    expect(fetch).toHaveBeenCalledTimes(1);
  });

  // TODO: test other cases, like 404 Http status and other network errors
});
