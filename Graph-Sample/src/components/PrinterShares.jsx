import React, { useState } from "react";
import Offcanvas from "react-bootstrap/Offcanvas";
import Form from "react-bootstrap/Form";
import Button from "react-bootstrap/Button";
import Alert from "react-bootstrap/Alert";
import Spinner from "react-bootstrap/Spinner";
import Table from "react-bootstrap/Table";

import { createPrintJob, uploadDocument } from "../graph";
import { useMsal } from "@azure/msal-react";
import { graphConfig, loginRequest } from "../authConfig";

const TableRow = ({row, onClick}) => {
    return (
        <tr onClick={onClick}>
            <td key={row.displayName}>{row.displayName}</td>
            <td key={row.id}>{row.model}</td>
            <td key={row.status.state}>{row.status.state}</td>
        </tr>
    );
};

const PrinterList = ({data}) => {
    const {instance, accounts} = useMsal();

    const [showPrinterDetails, setShowPrinterDetails] = useState(false);
    const [activeRow, setActiveRow] = useState(null);
    const [file, setFile] = useState(null);
    const [alert, setAlert] = useState(null);
    const [progress, setProgress] = useState(false);

    const showAlert = (type, msg, timeout=10000) => {
        setAlert({
            type,
            msg
        });
        setTimeout(() => setAlert(null), timeout);
    };

    const rowOnClick = (row) => {
        setActiveRow(row);
        setShowPrinterDetails(true);
    };

    const fileSelected = (event) => {
        if (event.target.files.length < 1) {
            return;
        }

        const file = event.target.files[0];

        if (file.type != "application/pdf") {
            showAlert("primary", <p>Universal Print supports both PDF and OXPS formats, but extending this sample to support OXPS is left as an exercise for the reader.</p>);
            return;
        }

        setFile(file);
    };

    const printButtonOnClick = (event) => {
        setProgress(true);
        var accessToken;
        instance.acquireTokenSilent({
            ...loginRequest,
            account: accounts[0]
        }).then((response) => {
            accessToken = response.accessToken;

            createPrintJob(response.accessToken, activeRow)
                .then((response) => {
                    if (response.hasOwnProperty("error")) {
                        console.error(response);
                        showAlert("danger", <code>{JSON.stringify(response)}</code>, 20000);
                        setProgress(false);
                    } else {
                        uploadDocument(accessToken, activeRow, response, file)
                            .then(response => response.json())
                            .then((response) => {
                                console.log(response);
                                showAlert("success", <p>The print job has been successfully queued.</p>, 20000);
                                setProgress(false);
                            });
                    }
                });
        }).catch(error => console.error(error));
    };
    return (
        <>
        <Table striped hover>
            <thead>
                <tr>
                    <th>Printer Share Name</th>
                    <th>Model</th>
                    <th>Status</th>
                </tr>
            </thead>
            <tbody>
                {data.map(row => (
                    <TableRow key={row.displayName} row={row} onClick={() => rowOnClick(row)} />
                    )
                )}
            </tbody>
        </Table>
        {showPrinterDetails &&
            <Offcanvas show={showPrinterDetails} onHide={() => {setShowPrinterDetails(false); setActiveRow(null)}} placement={"end"} name={activeRow.displayName}>
                <Offcanvas.Header closeButton>
                    <Offcanvas.Title>{activeRow.displayName}</Offcanvas.Title>
                </Offcanvas.Header>
                <Offcanvas.Body>
                    <p><b>Model: </b>{activeRow.model}</p>
                    {alert && <Alert variant={alert.type}>{alert.msg}</Alert> }
                    <Form.Group>
                        <Form.Label>Choose a document to print:</Form.Label>
                        <Form.Control type="file" onChange={fileSelected} />
                    </Form.Group>
                    <br />
                    <span>
                    <Button variant="primary" onClick={printButtonOnClick}>Print</Button>
                    {progress && <Spinner animation="border" variant="primary" />}
                    </span>
                    <br />
                </Offcanvas.Body>
            </Offcanvas>
        }
    </>
    );
};

export const PrinterShares = (props) => {
    return (
        <>
            {props.printerShareList ?
            <PrinterList data={props.printerShareList} />
            :
            <p>Loading printer shares...</p>
            }
        </>
    );
};