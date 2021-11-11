import { graphConfig } from "./authConfig";

/**
 * Attaches a given access token to a MS Graph API call. Returns information about the user
 * @param accessToken 
 */
export async function callMsGraph(accessToken) {
    const headers = new Headers();
    const bearer = `Bearer ${accessToken}`;

    headers.append("Authorization", bearer);

    const options = {
        method: "GET",
        headers: headers
    };

    return fetch(graphConfig.graphMeEndpoint, options)
        .then(response => response.json())
        .catch(error => console.log(error));
}

export async function listPrintShares(accessToken) {
    const headers = new Headers();
    const bearer = `Bearer ${accessToken}`;

    headers.append("Authorization", bearer);

    const options = {
        method: "GET",
        headers: headers
    };

    return fetch(graphConfig.graphPrintSharesEndpoint, options)
        .then(response => response.json())
        .catch(error => console.log(error));
}


// 1
export async function createPrintJob(accessToken, printer) {
    const headers = new Headers();
    const bearer = `Bearer ${accessToken}`;

    headers.append("Authorization", bearer);
    headers.append("Content-Type", "application/json");

    const endpoint = `${graphConfig.graphPrintEndpoint}/shares/${printer.id}/jobs`;

    const printJob = {
        configuration: {
            "@odata.type": "microsoft.graph.printJobConfiguration"
        }
    };

    const options = {
        method: "POST",
        body: JSON.stringify(printJob),
        headers: headers
    };

    return fetch(endpoint, options)
        .then(response => response.json())
        .catch(error => console.log(error));
}

// 2
async function createUploadSession(accessToken, printer, job, file) {
    const headers = new Headers();
    const bearer = `Bearer ${accessToken}`;

    headers.append("Authorization", bearer);
    headers.append("Content-Type", "application/json");

    const endpoint = `${graphConfig.graphPrintEndpoint}/shares/${printer.id}/jobs/${job.id}/documents/${job.documents[0].id}/createUploadSession`;

    const documentProperties = {
        properties: {
            contentType: file.type,
            documentName: file.name,
            size: file.size
        }
    };

    const options = {
        method: "POST",
        body: JSON.stringify(documentProperties),
        headers: headers
    };

    const uploadSession = (await fetch(endpoint, options)).json();
    return uploadSession;
}

// 3
export async function uploadDocument(accessToken, printer, job, file) {
    const uploadSession = await createUploadSession(accessToken, printer, job, file);

    /*
        For uploading large files, you would do something like the following:

        foreach range in uploadSession.expectedRanges
            PUT HTTP/1.1 uploadUrl
            Content-Range bytes range.start-range.end/file.size

            file.slice(range.start, range.end, file.type)

            fetch(...)
    */

    const uploadUrl = uploadSession.uploadUrl;
    const headers = new Headers();
    headers.append("Content-Range", `bytes 0-${file.size-1}/${file.size}`);

    const options = {
        method: "PUT",
        body: file.slice(0, file.size, file.type),
        headers: headers
    };

    const response = await fetch(uploadUrl, options);
    if (response.status == 201) {
        console.log("Document upload complete.");
        return await startPrintJob(accessToken, printer, job);
    } else {
        return response;
    }
}

// 4
async function startPrintJob(accessToken, printer, job) {
    const headers = new Headers();
    const bearer = `Bearer ${accessToken}`;

    headers.append("Authorization", bearer);

    const endpoint = `${graphConfig.graphPrintEndpoint}/shares/${printer.id}/jobs/${job.id}/start`;

    const options = {
        method: "POST",
        headers: headers
    }

    return await fetch(endpoint, options);
}