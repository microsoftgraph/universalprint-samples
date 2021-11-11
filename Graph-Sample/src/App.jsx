import React, { useState } from "react";
import { AuthenticatedTemplate, UnauthenticatedTemplate, useMsal } from "@azure/msal-react";
import { loginRequest } from "./authConfig";
import { PageLayout } from "./components/PageLayout";
import { ProfileData } from "./components/ProfileData";
import { callMsGraph, listPrintShares } from "./graph";
import Button from "react-bootstrap/Button";
import "./styles/App.css";
import { PrinterShares } from "./components/PrinterShares";
import { Container } from "react-bootstrap";

const printerSharesStorageKey = "printerSharesJson";

/**
 * Renders information about the signed-in user or a button to retrieve data about the user
 */
const ProfileContent = () => {
    const { instance, accounts } = useMsal();
    const [printerShareList, setPrinterShareList] = useState(null);

    function RequestProfileData() {
        if (!window.sessionStorage.getItem(printerSharesStorageKey)) {
            // Silently acquires an access token which is then attached to a request for MS Graph data
            instance.acquireTokenSilent({
                ...loginRequest,
                account: accounts[0]
            }).then((response) => {
                //debugger
                listPrintShares(response.accessToken).then(
                    response => {
                        setPrinterShareList(response["value"]);
                        window.sessionStorage.setItem(printerSharesStorageKey, JSON.stringify(response["value"]));
                    });
            });
        } else {
            setPrinterShareList(JSON.parse(window.sessionStorage.getItem(printerSharesStorageKey)));
        }
    }

    return (
        <>
            <h5 className="card-title">Hi, {accounts[0].name}</h5>
            {printerShareList ? 
                <center>
                <PrinterShares printerShareList={printerShareList} />
                </center>
                :
                <Button variant="secondary" onClick={RequestProfileData}>Load Printers</Button>
            }
        </>
    );
};

/**
 * If a user is authenticated the ProfileContent component above is rendered. Otherwise a message indicating a user is not authenticated is rendered.
 */
const MainContent = () => {    
    return (
        <div className="App">
            <Container>
                <AuthenticatedTemplate>
                    <ProfileContent />
                </AuthenticatedTemplate>

                <UnauthenticatedTemplate>
                    <h5 className="card-title">Sign in to see your printers.</h5>
                </UnauthenticatedTemplate>
            </Container>
        </div>
    );
};

export default function App() {
    return (
        <PageLayout>
            <MainContent />
        </PageLayout>
    );
}
