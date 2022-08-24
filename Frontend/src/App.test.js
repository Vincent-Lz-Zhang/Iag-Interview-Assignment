/**/
import React from "react";
import { createRoot } from "react-dom/client";
import App from "./App";

it("should render without crashing", () => {
  const container = document.createElement("div");
  document.body.appendChild(container);
  const root = createRoot(container);
  root.render(<App />);
  root.unmount();
});
