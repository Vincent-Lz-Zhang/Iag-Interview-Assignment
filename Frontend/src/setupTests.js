// jest-dom adds custom jest matchers for asserting on DOM nodes.
// allows you to do things like:
// expect(element).toHaveTextContent(/react/i)
// learn more: https://github.com/testing-library/jest-dom
import "@testing-library/jest-dom";
import fetchMock from "jest-fetch-mock";

fetchMock.enableMocks();

global.generateRandomString = function (length) {
  return [...Array(length)].map(() => Math.random().toString(36)[2]).join("");
};

global.generateRandomInteger = function (min, max) {
  return Math.floor(min + Math.random() * (max - min + 1));
};
