// Sample patient data
const patients = [
    { id: "#40322", name: "Kathryn Murphy", disease: "Diabetes", status: "Discharged" },
    { id: "#40323", name: "Esther Howard", disease: "Hypertension", status: "Transferred" },
    { id: "#40324", name: "Cameron Williamson", disease: "Hepatitis C", status: "Recovered" },
    { id: "#40325", name: "Darlene Robertson", disease: "Measles", status: "In Treatment" },
    { id: "#40326", name: "Marvin McKinney", disease: "Influenza", status: "New Patient" },
  ];
  
  // Render the list of patients
  function renderPatients() {
    const patientList = document.getElementById("patientList");
    patientList.innerHTML = patients
      .map(
        (patient) => `
        <div class="patient-card">
          <div>
            <strong>${patient.id} - ${patient.name}</strong>
            <p>Disease: ${patient.disease}</p>
            <p>Status: ${patient.status}</p>
          </div>
        </div>
      `
      )
      .join("");
  }
  
  // Initial rendering of patients
  renderPatients();
  