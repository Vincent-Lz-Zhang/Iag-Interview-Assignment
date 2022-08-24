/// <reference types="cypress" />

describe("vehicle summary component test suite", () => {
  it("should display a list of vehicle models", () => {
    cy.visit("http://localhost:3000");
    cy.get("[data-cy=make-text-box]").type("Lotus");
    cy.get("[data-cy=query-button]").click();
    cy.get("tr[data-cy='vehicle-model-row']").its("length").should("be.gte", 1);
  });
  // TODO: mock the backend API
  // TODO: test other cases
});
