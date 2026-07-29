-- Shared Kernel baseline constraint script for EP-0-1.
-- This script mirrors the EF Core model constraints in AcademicConfiguration.

CREATE UNIQUE INDEX UX_Academics_EmpNr
    ON Academics(EmpNr);

CREATE UNIQUE INDEX UX_Academics_ExtensionNumber
    ON Academics(ExtensionNumber);

ALTER TABLE Academics
    ADD CONSTRAINT CK_Academics_EmploymentState
    CHECK (NOT (IsTenured = 1 AND ContractEndDate IS NOT NULL));

CREATE UNIQUE INDEX UX_AcademicQualifications_AcademicEmpNr_DegreeCode
    ON AcademicQualifications(AcademicEmpNr, DegreeCode);
