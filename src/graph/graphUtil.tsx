// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT license. 

import { hostUrl, IGraphPrinterSharesResponse, IPrinterShare, IPrintJobStatus, IPrintJob, IDocumentConfiguration, IPrintDocument } from "./graphModel";


/** Invokes List Print Shares Microsoft Graph API and returns list of user PrinterShares.
 * @returns List of printershares objects.
 */
export const getPrinterShares = async (authToken: string): Promise<IPrinterShare[]> => {
    const listPrintsRequestInit = {
        method: 'GET',
        mode: 'cors' as RequestMode,
        headers: {
            'Accept': 'application/json',
            'Authorization': 'Bearer ' + authToken
        },
    };

    const url = `${hostUrl}/print/shares?$select=id,name,defaults,capabilities`;

    const response = await fetch(url, listPrintsRequestInit) as Response;
    const value = await response.json() as IGraphPrinterSharesResponse;
    const printShares = value.value as IPrinterShare[];
    return printShares;
}

/** Invokes CreatePrintJob Microsoft Graph API and returns a IPrintJob object
 *  @return IPrintJob which includes JobID and document Id created by universal print 
 * */
export const createJob = async (printerId: string, sizeInBytes: number, documentConfig: IDocumentConfiguration, authToken: string): Promise<IPrintJob> => {
    let url = `${hostUrl}/print/shares/${printerId}/jobs`;

    const printDocument = {
        name: 'test',
        mimeType: 'application/oxps',
        sizeInBytes: sizeInBytes,
        documentConfiguration: documentConfig
    } as IPrintDocument;

    const printJob = {
        documents: [printDocument]
    } as IPrintJob;

    let requestInit = {
        method: 'POST',
        mode: 'cors' as RequestMode,
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Authorization': 'Bearer ' + authToken
        },
        body: JSON.stringify(printJob)
    } as RequestInit;

    let response = await fetch(url, requestInit);
    if (response.ok) {
        return response.json();
    } else {
        throw new Error(response.statusText);
    }
}

/** Invokes Upload Document Microsoft Graph API 
 * @returns Response. if successful it will 201/202 response codes */
export const uploadData = async (printerId: string, job: IPrintJob, authToken: string, array: Uint8Array): Promise<Response> => {

    const url = `${hostUrl}/print/shares/${printerId}/jobs/${job.id}/documents/${job.documents[0].id}/uploadData`;

    const requestInit = {
        method: 'POST',
        mode: 'cors' as RequestMode,
        headers: {
            'Accept': 'application/json',
            'Authorization': 'Bearer ' + authToken,
            'Range': `bytes=0-${array.length - 1}`
        },
        body: array
    } as RequestInit;

    const response = await fetch(url, requestInit);
    if (response.ok) {
        return Promise.resolve(response);
    } else {
        throw new Error(response.statusText);
    }
}

/** Invokes Start Print Job Microsoft Graph API to notify universal print to start printing
 * @returns IPrintJobStatus which contains the current status of the job
 */
export const startPrintJob = async (printerId: string, jobId: string, authToken: string): Promise<IPrintJobStatus> => {

    const url = `${hostUrl}/print/shares/${printerId}/jobs/${jobId}/startPrintJob`;
    const requestInit = {
        method: 'POST',
        mode: 'cors' as RequestMode,
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            'Authorization': 'Bearer ' + authToken
        }
    } as RequestInit;

    const response = await fetch(url, requestInit);
    const jobStatus = await response.json();
    return jobStatus;
}