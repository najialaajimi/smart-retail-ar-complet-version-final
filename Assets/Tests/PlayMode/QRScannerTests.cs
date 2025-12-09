using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SmartRetailAR.QRCode;

namespace SmartRetailAR.Tests.PlayMode
{
    /// <summary>
    /// Tests PlayMode pour le scanner QR
    /// </summary>
    public class QRScannerTests
    {
        private GameObject scannerObject;
        private QRCodeScanner scanner;

        [SetUp]
        public void Setup()
        {
            scannerObject = new GameObject("TestQRScanner");
            scanner = scannerObject.AddComponent<QRCodeScanner>();
        }

        [TearDown]
        public void Teardown()
        {
            if (scannerObject != null)
            {
                Object.Destroy(scannerObject);
            }
        }

        [UnityTest]
        public IEnumerator SimulateScan_ValidQRData_TriggersEvent()
        {
            // Arrange
            bool eventTriggered = false;
            string scannedData = null;

            scanner.onQRCodeScanned.AddListener((data) =>
            {
                eventTriggered = true;
                scannedData = data;
            });

            scanner.StartScanning();
            yield return null;

            // Act
            scanner.SimulateScan("SMARTRETAIL:PROD001");
            yield return new WaitForSeconds(0.1f);

            // Assert
            Assert.IsTrue(eventTriggered, "L'événement n'a pas été déclenché");
            Assert.AreEqual("SMARTRETAIL:PROD001", scannedData);
        }

        [UnityTest]
        public IEnumerator StartScanning_InitializesScanner()
        {
            // Act
            scanner.StartScanning();
            yield return new WaitForSeconds(0.1f);

            // Assert
            Assert.IsTrue(scanner.IsScanning());
        }

        [UnityTest]
        public IEnumerator StopScanning_StopsScanner()
        {
            // Arrange
            scanner.StartScanning();
            yield return new WaitForSeconds(0.1f);

            // Act
            scanner.StopScanning();
            yield return new WaitForSeconds(0.1f);

            // Assert
            Assert.IsFalse(scanner.IsScanning());
        }
    }
}
