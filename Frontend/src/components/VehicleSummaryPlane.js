import React, { useState } from "react";
import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import Paper from "@mui/material/Paper";
import Input from "@mui/material/Input";
import Button from "@mui/material/Button";
import fetchVehicleSummary from "../modules/fetchVehicleSummary.js";
import loadingImg from "../img/loading.gif";

const VehicleSummaryPlane = () => {
  const [make, setMake] = useState("");
  const [vehicleSummary, setVehicleSummary] = useState({});
  const [isLoading, setIsLoading] = useState(false);

  const onQuery = async (e) => {
    if (!make) {
      alert("Please input a make"); // TODO: use a better prompt component with m-UI
      return;
    } else {
      setIsLoading(true);
    }
    try {
      const vehicleSummaryResponse = await fetchVehicleSummary(make);
      if (vehicleSummaryResponse) {
        setVehicleSummary(vehicleSummaryResponse);
        setIsLoading(false);
      }
    } catch (error) {
      setIsLoading(false);
      alert(error.message); // TODO: use a better prompt with m-UI
    }

    setMake("");
  };

  const formContainerStyle = {
    paddingTop: "5px",
  };

  return (
    <div>
      <AppBar position="static">
        <Toolbar variant="dense">
          <Typography variant="h6" color="inherit">
            Vehicle List
          </Typography>
        </Toolbar>
      </AppBar>
      <div style={formContainerStyle}>
        <Input
          type="text"
          placeholder="Input the make here..."
          value={make}
          onChange={(e) => setMake(e.target.value)}
          inputProps={{ "data-cy": "make-text-box" }}
        />
        <Button
          variant="contained"
          data-cy="query-button"
          style={{ marginLeft: 4 }}
          onClick={onQuery}
        >
          Query
        </Button>
      </div>

      <Paper>
        <Table aria-label="simple table">
          <TableHead>
            <TableRow>
              <TableCell component="th" scope="row">
                <h4>Make: {vehicleSummary?.make}</h4>
              </TableCell>
            </TableRow>
          </TableHead>

          {isLoading ? (
            <TableBody>
              <TableRow>
                <TableCell>
                  <img
                    style={{ height: "180px" }}
                    src={loadingImg}
                    alt="Loading..."
                  ></img>
                </TableCell>
              </TableRow>
            </TableBody>
          ) : vehicleSummary?.models?.length ?? 0 > 0 ? (
            <TableBody>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>Years Available</TableCell>
              </TableRow>
              {vehicleSummary.models.map((model, index) => (
                <TableRow
                  data-cy="vehicle-model-row"
                  key={model.name + model.yearsAvailable}
                >
                  <TableCell component="th" scope="row">
                    {model.name}
                  </TableCell>
                  <TableCell>{model.yearsAvailable}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          ) : (
            <TableBody>
              <TableRow>
                <TableCell component="th" scope="row">
                  No Model To Show
                </TableCell>
              </TableRow>
            </TableBody>
          )}
        </Table>
      </Paper>
    </div>
  );
};

export default VehicleSummaryPlane;
