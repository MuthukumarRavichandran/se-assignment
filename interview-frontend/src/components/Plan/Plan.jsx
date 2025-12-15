import { useState, useEffect, useRef } from "react";
import { useParams } from "react-router-dom";
import {
  addProcedureToPlan,
  removeProcedureFromPlan,
  getPlanAndAssignedUsers,
  getProcedures,
  getUsers
} from "../../api/api";
import Layout from '../Layout/Layout';
import ProcedureItem from "./ProcedureItem/ProcedureItem";
import PlanProcedureItem from "./PlanProcedureItem/PlanProcedureItem";

const Plan = () => {
  let { id } = useParams();
  const [procedures, setProcedures] = useState([]);
  const [planProcedures, setPlanProcedures] = useState([]);
  const [users, setUsers] = useState([]);

  const didFetch = useRef(false);

  useEffect(() => {
    if (didFetch.current) return;
    didFetch.current = true;

    (async () => {
      try {
        var procedures = await getProcedures();
        //var planProcedureUsers = await getPlanProcedures(id);
        var planAndAssignedUserData = await getPlanAndAssignedUsers(id);
        var users = await getUsers();

        var userOptions = [];
        users.map((u) => userOptions.push({ label: u.name, value: u.userId }));

        setUsers(userOptions);
        setProcedures(procedures);
        setPlanProcedures(planAndAssignedUserData.planProcedures);
      }
      catch(err){
        alert(err);
      }
    })();
  }, [id]);

  const handleToggleProcedure = async (procedure, isChecked) => {

  if (isChecked) {
    await removeProcedureFromPlan(id, procedure.procedureId);

    setPlanProcedures(prev =>
      prev.filter(p => p.procedureId !== procedure.procedureId)
    );

    return;
  }
  const hasProcedureInPlan = planProcedures.some((p) => p.procedureId === procedure.procedureId);
    if (hasProcedureInPlan) return;

    await addProcedureToPlan(id, procedure.procedureId);
    setPlanProcedures(prevState => [
      ...prevState,
      {
        planId: id,
        procedureId: procedure.procedureId,
        procedure,
        assignedUsers: []
      }
    ]);
};

  return (
    <Layout>
      <div className="container pt-4">
        <div className="d-flex justify-content-center">
          <h2>OEC Interview Frontend</h2>
        </div>
        <div className="row mt-4">
          <div className="col">
            <div className="card shadow">
              <h5 className="card-header">Repair Plan</h5>
              <div className="card-body">
                <div className="row">
                  <div className="col">
                    <h4>Procedures</h4>
                    <div>
                    {procedures.map((p) => (
                      <ProcedureItem
                        key={p.procedureId}
                        procedure={p}
                        planProcedures={planProcedures}
                        handleToggleProcedure={handleToggleProcedure}
                      />
                    ))}
                    </div>
                  </div>
                  <div className="col">
                    <h4>Added to Plan</h4>
                    <div>
                    {planProcedures.map((p) => (
                      <PlanProcedureItem
                        key={p.procedureId}
                        planId={id}
                        procedure={p.procedure}
                        users={users}
                        assignedUsers={p.assignedUsers}
                      />
                    ))}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default Plan;
