// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Graph;
using System;
using System.Threading.Tasks;

namespace S2SIntegration_CreateGraphEntities
{
    public class PrinterRegistrationHelper
    {
        private GraphSdkClient graphClient;

        public PrinterRegistrationHelper(GraphSdkClient graphClient)
        {
            this.graphClient = graphClient;
        }

        public async Task PrinterRegistrationDemoAsync()
        {
            /*
             * Operation 1: Registering a Printer (with base attributes)
             */

            var newRegisteredPrinter = await RegisterPrinterAsync(string.Format(CommonConstants.PrinterNameFormat, DateTime.UtcNow.Ticks));

            // Displaying Printer information ( Printer Name and a few printer defaults)
            DisplayPrinterInformation(newRegisteredPrinter);

            /*
             * Operation 2: Adding additional printer paramaters and updating the printer status
             * For Windows clients, to print to a printer successfully, the printer must have Mopria 2.0 required attributes.
             */
            await UpdatePrinterAsync(newRegisteredPrinter.Id, CommonConstants.PrinterAttributeBinLocation);

            // Get Printer after the update and display the properties
            var printer = await GetPrinterAsync(newRegisteredPrinter.Id);
            DisplayPrinterInformation(printer);
        }

        public async Task<Printer> RegisterPrinterAsync(string printerName)
        {
            /*
             * STEP 1: Initiate Printer Registration
             */
            var initiateRegisterPrinterResponse = await this.graphClient.RegisterPrinterAsync(printerName);

            /*
             * STEP 2: Poll the Registration Status
             */
            var registeredPrinter = await this.graphClient.GetRegisteredPrinterAsync(initiateRegisterPrinterResponse);

            return registeredPrinter;
        }

        public async Task<Printer> GetPrinterAsync(string printerId)
        {
            return await this.graphClient.GetPrinterAsync(printerId);
        }

        public async Task UpdatePrinterAsync(string printerId, string locationToAttributeFile)
        {
            /*
             * STEP 1: Reading the printer attributes stored in the binary file. The binary file contains a "PrinterAttributes" Group required by Mopria specification.
             * Refer to https://datatracker.ietf.org/doc/html/rfc8010 for more information about how the binary file is encoded.
             * Note: The payload should only contain IPP PrinterAttributes Group and should not contain IPP OperationAttributes group.
             */

            byte[] printerAttributesBinaryData = System.IO.File.ReadAllBytes(locationToAttributeFile);
            await this.graphClient.UpdatePrinterAsync(printerId, printerAttributesBinaryData);
        }

        public void DisplayPrinterInformation(Printer printer)
        {
            Console.WriteLine($"====================================================================");
            Console.WriteLine($"Printer Name: {printer.DisplayName}");
            Console.WriteLine($"Printer Id: {printer.Id}");
            Console.WriteLine($"Printer Model: {printer.Model}");
            Console.WriteLine($"Printer Manufacturer: {printer.Manufacturer}");
            if (printer.Defaults != null)
            {
                Console.WriteLine("=================");
                Console.WriteLine($"Printer Defaults");
                Console.WriteLine("=================");
                Console.WriteLine($"Copied Per Job: {printer.Defaults.CopiesPerJob}");
                Console.WriteLine($"Content Type: {printer.Defaults.ContentType}");
                Console.WriteLine($"Finishings: {printer.Defaults.Finishings}");
                Console.WriteLine($"Media Type: {printer.Defaults.MediaType}");
                Console.WriteLine($"Media Size: {printer.Defaults.MediaSize}");
                Console.WriteLine($"Input Bin: {printer.Defaults.InputBin}");
                Console.WriteLine($"Output Bin: {printer.Defaults.OutputBin}");
                Console.WriteLine($"Color Mode: {printer.Defaults.ColorMode}");
                Console.WriteLine($"Quality: {printer.Defaults.Quality}");
                Console.WriteLine($"Duplex Mode: {printer.Defaults.DuplexMode}");
                Console.WriteLine($"DPI: {printer.Defaults.Dpi}");
                Console.WriteLine($"Scaling: {printer.Defaults.Scaling}");
            }
            Console.WriteLine($"====================================================================");

            Console.WriteLine();
        }
    }
}
