using System;

using HealthTrackPro.Web.Enums;
using HealthTrackPro.Web.Models.Communication;
using HealthTrackPro.Web.Models.Diet;
using HealthTrackPro.Web.Models.Fitness;
using HealthTrackPro.Web.Models.Scheduling;
using HealthTrackPro.Web.Models.Workouts;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HealthTrackPro.Web.Tests.Domain
{
    [TestClass]
    public class DomainModelTests
    {
        // =========================================================
        // HEALTH METRICS
        // =========================================================

        [TestMethod]
        public void HealthMetric_CalculateBMI_WithValidValues_ReturnsExpectedBMI()
        {
            var metric =
                new HealthMetric
                {
                    WeightKg = 70m,
                    HeightCm = 175m
                };

            var result =
                metric.CalculateBMI();

            Assert.IsTrue(result.HasValue);

            Assert.AreEqual(
                22.86m,
                Math.Round(result.Value, 2));
        }

        [TestMethod]
        public void HealthMetric_CalculateBMI_WithoutWeight_ReturnsNull()
        {
            var metric =
                new HealthMetric
                {
                    HeightCm = 175m
                };

            var result =
                metric.CalculateBMI();

            Assert.IsFalse(result.HasValue);
        }

        [TestMethod]
        public void HealthMetric_CalculateBMI_WithoutHeight_ReturnsNull()
        {
            var metric =
                new HealthMetric
                {
                    WeightKg = 70m
                };

            var result =
                metric.CalculateBMI();

            Assert.IsFalse(result.HasValue);
        }


        // =========================================================
        // FITNESS GOALS
        // =========================================================

        [TestMethod]
        public void FitnessGoal_UpdateCurrentValue_ChangesCurrentValue()
        {
            var goal =
                new FitnessGoal
                {
                    CurrentValue = 80m
                };

            goal.UpdateCurrentValue(75m);

            Assert.AreEqual(
                75m,
                goal.CurrentValue);
        }

        [TestMethod]
        public void FitnessGoal_MarkAsCompleted_SetsCompletedState()
        {
            var goal =
                new FitnessGoal();

            goal.MarkAsCompleted();

            Assert.IsTrue(
                goal.IsCompleted);
        }


        // =========================================================
        // WORKOUT PLAN
        // =========================================================

        [TestMethod]
        public void WorkoutPlan_Activate_SetsPlanActive()
        {
            var plan =
                new WorkoutPlan();

            plan.Activate();

            Assert.IsTrue(
                plan.IsActive);
        }

        [TestMethod]
        public void WorkoutPlan_Deactivate_SetsPlanInactive()
        {
            var plan =
                new WorkoutPlan();

            plan.Activate();
            plan.Deactivate();

            Assert.IsFalse(
                plan.IsActive);
        }


        // =========================================================
        // DIET PLAN
        // =========================================================

        [TestMethod]
        public void DietPlan_Activate_SetsPlanActive()
        {
            var plan =
                new DietPlan();

            plan.Activate();

            Assert.IsTrue(
                plan.IsActive);
        }

        [TestMethod]
        public void DietPlan_Deactivate_SetsPlanInactive()
        {
            var plan =
                new DietPlan();

            plan.Activate();
            plan.Deactivate();

            Assert.IsFalse(
                plan.IsActive);
        }


        // =========================================================
        // NOTIFICATION
        // =========================================================

        [TestMethod]
        public void Notification_MarkAsRead_SetsReadState()
        {
            var notification =
                new Notification();

            notification.MarkAsRead();

            Assert.IsTrue(
                notification.IsRead);
        }

        [TestMethod]
        public void Notification_MarkAsRead_SetsReadTimestamp()
        {
            var notification =
                new Notification();

            notification.MarkAsRead();

            Assert.IsTrue(
                notification.ReadAt.HasValue);
        }


        // =========================================================
        // APPOINTMENTS
        // =========================================================

        [TestMethod]
        public void Appointment_Schedule_SetsDateAndDuration()
        {
            var appointment =
                new Appointment();

            var appointmentDate =
                DateTime.Now.AddDays(2);

            appointment.Schedule(
                appointmentDate,
                60);

            Assert.AreEqual(
                appointmentDate,
                appointment.AppointmentDateTime);

            Assert.AreEqual(
                60,
                appointment.DurationMinutes);
        }

        [TestMethod]
        public void Appointment_Schedule_SetsStatusToPending()
        {
            var appointment =
                new Appointment();

            appointment.Schedule(
                DateTime.Now.AddDays(2),
                45);

            Assert.AreEqual(
                AppointmentStatus.Pending,
                appointment.Status);
        }

        [TestMethod]
        public void Appointment_Approve_PendingAppointment_SetsApproved()
        {
            var appointment =
                new Appointment();

            appointment.Schedule(
                DateTime.Now.AddDays(2),
                60);

            appointment.Approve();

            Assert.AreEqual(
                AppointmentStatus.Approved,
                appointment.Status);
        }

        [TestMethod]
        public void Appointment_Reject_PendingAppointment_SetsRejected()
        {
            var appointment =
                new Appointment();

            appointment.Schedule(
                DateTime.Now.AddDays(2),
                60);

            appointment.Reject();

            Assert.AreEqual(
                AppointmentStatus.Rejected,
                appointment.Status);
        }

        [TestMethod]
        public void Appointment_Cancel_ScheduledAppointment_SetsCancelled()
        {
            var appointment =
                new Appointment();

            appointment.Schedule(
                DateTime.Now.AddDays(2),
                60);

            appointment.Cancel();

            Assert.AreEqual(
                AppointmentStatus.Cancelled,
                appointment.Status);
        }

        [TestMethod]
        public void Appointment_Reschedule_ChangesAppointmentDate()
        {
            var appointment =
                new Appointment();

            var originalDate =
                DateTime.Now.AddDays(2);

            var newDate =
                DateTime.Now.AddDays(4);

            appointment.Schedule(
                originalDate,
                60);

            appointment.Reschedule(
                newDate);

            Assert.AreEqual(
                newDate,
                appointment.AppointmentDateTime);
        }

        [TestMethod]
        public void Appointment_Complete_ApprovedAppointment_SetsCompleted()
        {
            var appointment =
                new Appointment();

            appointment.Schedule(
                DateTime.Now.AddDays(2),
                60);

            appointment.Approve();
            appointment.Complete();

            Assert.AreEqual(
                AppointmentStatus.Completed,
                appointment.Status);
        }
    }
}
