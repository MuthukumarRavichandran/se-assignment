import { useState,useEffect } from "react";
import ReactSelect from "react-select";
import { assignUser, unassignUser, clearAssignedUsers } from "../../../api/api";

const PlanProcedureItem = ({ planId, procedure, users, assignedUsers }) => {
  const [selectedUsers, setSelectedUsers] = useState([]);

  useEffect(() => {
    const mappedUser = users.filter(u =>
      assignedUsers.some(au => au.userId === u.value)
    );
    setSelectedUsers(mappedUser);
  }, [assignedUsers, users]);

  const handleAssignUserToProcedure = async (newValue, actionMeta) => {

    if (actionMeta.action === "clear") {
      if (selectedUsers.length > 0) {
        await clearAssignedUsers(planId, procedure.procedureId);
      }
      setSelectedUsers([]);
      return;
    }

    const newUserIds = newValue.map(u => u.value);
    const oldUserIds = selectedUsers.map(u => u.value);

    const added = newUserIds.filter(id => !oldUserIds.includes(id));
    const removed = oldUserIds.filter(id => !newUserIds.includes(id));

    for (const userId of added) {
      await assignUser(planId, procedure.procedureId, userId);
    }

    for (const userId of removed) {
      await unassignUser(planId, procedure.procedureId, userId);
    }

    setSelectedUsers(newValue);
  };

  return (
    <div className="py-2">
      <div>
        {procedure.procedureTitle}
        </div>

      <ReactSelect
        className="mt-2"
        placeholder="Select User to Assign"
        isMulti={true}
        isClearable={true}
        options={users}
        value={selectedUsers}
        onChange={handleAssignUserToProcedure}
      />
    </div>
  );
};

export default PlanProcedureItem;
