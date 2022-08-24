import { render, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom/extend-expect";
import VehicleSummaryPlane from "./VehicleSummaryPlane";

describe("VehicleSummaryPlane Component", () => {
  it("should render make text box", () => {
    const { debug, getByRole } = render(<VehicleSummaryPlane />);
    //debug();
    expect(getByRole("textbox")).toBeInTheDocument();
  });
  it("should render query button", () => {
    const { getByRole } = render(<VehicleSummaryPlane />);
    expect(getByRole("button")).toBeInTheDocument();
  });
  it("should render preloading gif when user types some text and hit query button", () => {
    const { getByRole, getByAltText } = render(<VehicleSummaryPlane />);
    const makeTextBox = getByRole("textbox");
    const queryButton = getByRole("button");
    const arbitraryMake = global.generateRandomString(10);
    fireEvent.change(makeTextBox, {
      target: { value: arbitraryMake },
    });
    fireEvent.click(queryButton);
    expect(getByAltText("Loading...")).toBeInTheDocument();
  });

  // TODO: test other cases, like error occurring
});
