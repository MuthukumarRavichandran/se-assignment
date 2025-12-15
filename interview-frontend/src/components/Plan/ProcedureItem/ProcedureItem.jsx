const ProcedureItem = ({ procedure, handleToggleProcedure, planProcedures }) => {

  const isChecked = planProcedures.some(
    p => p.procedureId === procedure.procedureId
  );

  return (
    <div className="py-2">
      <div className="form-check">
        <input
          className="form-check-input"
          type="checkbox"
          checked={isChecked}
          onChange={() => handleToggleProcedure(procedure, isChecked)}
        />
        <label className="form-check-label" htmlFor="procedureCheckbox">
            {procedure.procedureTitle}
        </label>
      </div>
    </div>
  );
};

export default ProcedureItem;
