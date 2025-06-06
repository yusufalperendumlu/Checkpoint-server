﻿using Checkpoint.MailService.Interfaces;

namespace Checkpoint.MailService.BackgroundJobs
{
    public class NotSentMailJob(IMailDbContext mailDbContext, MailServices.MailService mailService)
    {
        public async Task ExecuteJob(CancellationToken cancellationToken)
        {
            var NotProcesseds = mailDbContext.NotSentMail.Where(y => !y.Processed).ToList();

            foreach (var notProcessed in NotProcesseds)
            {
                try
                {
                    await mailService.SendEmailAsync(notProcessed.Email, "Verification", notProcessed.VerificationCode);
                    notProcessed.Processed = true;
                    await mailDbContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception)
                {
                    continue;
                }

            }
        }
    }
}
